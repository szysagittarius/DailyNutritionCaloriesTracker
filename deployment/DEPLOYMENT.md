# Azure Deployment Guide

This guide explains how to deploy the Nutrition Tracker application to Azure using Azure Functions and Azure Table Storage.

## Architecture Overview

- **Azure Functions** (Consumption Plan): Serverless API endpoints
- **Azure Table Storage**: NoSQL data persistence
- **Cost**: ~$0.05/month (vs $18-50/month for SQL Server + App Service)

## Prerequisites

1. Azure subscription
2. GitHub account with repository access
3. Azure CLI installed locally (optional, for manual deployment)

## Pipeline Structure

All pipeline files are located in the `deployment/` folder for better organization:

```
deployment/
├── pipelines/              # CI/CD pipeline YAML files
│   ├── build-and-test.yml              # GitHub Actions
│   ├── infrastructure.yml               # GitHub Actions
│   ├── deploy-azure-functions.yml      # GitHub Actions
│   ├── azure-devops-build.yml          # Azure DevOps
│   ├── azure-devops-infrastructure.yml # Azure DevOps
│   └── azure-devops-deploy.yml         # Azure DevOps
└── scripts/                # Deployment helper scripts
    ├── create-service-principal.sh
    ├── deploy-infrastructure.sh
    └── deploy-application.sh
```

This structure works with both **GitHub Actions** and **Azure DevOps Pipelines**.

## Deployment Options

### Option 1: Automated Deployment via GitHub Actions (Recommended)

#### Step 1: Create Azure Resources

1. Go to GitHub repository → Actions
2. Run workflow: **"Deploy Infrastructure"**
3. Select environment (dev/staging/production)
4. Click "Run workflow"

This creates:
- Resource Group
- Azure Storage Account (for Table Storage)
- Azure Function App (Consumption Plan - FREE tier)
- Storage Account for Functions runtime

#### Step 2: Configure GitHub Secrets

After infrastructure deployment, add these secrets to GitHub:

**Repository Settings → Secrets and variables → Actions**

1. **AZURE_CREDENTIALS**: Service Principal credentials
   ```json
   {
     "clientId": "<client-id>",
     "clientSecret": "<client-secret>",
     "subscriptionId": "<subscription-id>",
     "tenantId": "<tenant-id>"
   }
   ```

2. **AZURE_FUNCTIONAPP_PUBLISH_PROFILE**: 
   - Download from workflow artifacts or Azure Portal
   - Function App → Get publish profile
   - Copy entire XML content

3. **AZURE_RESOURCE_GROUP**: Resource group name (e.g., `rg-nutrition-tracker-dev`)

#### Step 3: Deploy Application Code

1. Push to `main` branch, OR
2. Run workflow: **"Deploy Azure Functions"** manually from Actions tab

The pipeline automatically:
- Builds the solution
- Runs tests
- Publishes Azure Functions
- Deploys to Azure

### Option 1b: Azure DevOps Pipelines

If using Azure DevOps instead of GitHub Actions:

1. **Create Build Pipeline**: Point to `deployment/pipelines/azure-devops-build.yml`
2. **Create Infrastructure Pipeline**: Point to `deployment/pipelines/azure-devops-infrastructure.yml`
3. **Create Deploy Pipeline**: Point to `deployment/pipelines/azure-devops-deploy.yml`
4. **Configure Service Connection**: Azure Resource Manager connection with Contributor role
5. **Set Variables**: 
   - `azureServiceConnection` - Name of your Azure service connection

See `deployment/README.md` for detailed Azure DevOps setup.

### Option 2: Manual Deployment via Scripts

Use the provided deployment scripts for quick manual deployment:

```bash
# Navigate to deployment scripts
cd deployment/scripts

# 1. Create service principal (one-time setup)
./create-service-principal.sh

# 2. Deploy infrastructure
./deploy-infrastructure.sh dev eastus

# 3. Deploy application
./deploy-application.sh dev
```

### Option 3: Manual Deployment via Azure CLI

#### Prerequisites
```bash
# Install Azure CLI
# https://docs.microsoft.com/en-us/cli/azure/install-azure-cli

# Login to Azure
az login

# Set subscription
az account set --subscription "YOUR_SUBSCRIPTION_NAME"
```

#### Step 1: Create Resources
```bash
# Variables
RESOURCE_GROUP="rg-nutrition-tracker-dev"
LOCATION="eastus"
STORAGE_ACCOUNT="stnutritiontrackerdev"
FUNCTION_APP="func-nutrition-tracker-dev"
FUNC_STORAGE="stfuncnutritiondev"

# Create resource group
az group create --name $RESOURCE_GROUP --location $LOCATION

# Create storage account for Table Storage
az storage account create \
  --name $STORAGE_ACCOUNT \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --sku Standard_LRS \
  --kind StorageV2

# Create storage account for Function App
az storage account create \
  --name $FUNC_STORAGE \
  --resource-group $RESOURCE_GROUP \
  --location $LOCATION \
  --sku Standard_LRS

# Create Function App (Consumption Plan - FREE)
az functionapp create \
  --name $FUNCTION_APP \
  --resource-group $RESOURCE_GROUP \
  --consumption-plan-location $LOCATION \
  --storage-account $FUNC_STORAGE \
  --runtime dotnet-isolated \
  --runtime-version 8 \
  --functions-version 4 \
  --os-type Linux

# Get Table Storage connection string
TABLE_CONNECTION=$(az storage account show-connection-string \
  --name $STORAGE_ACCOUNT \
  --resource-group $RESOURCE_GROUP \
  --query connectionString -o tsv)

# Configure Function App settings
az functionapp config appsettings set \
  --name $FUNCTION_APP \
  --resource-group $RESOURCE_GROUP \
  --settings "AzureTableStorageConnectionString=$TABLE_CONNECTION"

# Enable CORS (adjust allowed origins for production)
az functionapp cors add \
  --name $FUNCTION_APP \
  --resource-group $RESOURCE_GROUP \
  --allowed-origins "*"
```

#### Step 2: Deploy Code
```bash
# Navigate to Azure Functions project
cd src/Adapters/Input/NutritionTracker.AzureFunctions

# Build and publish
dotnet publish --configuration Release --output ./publish

# Deploy to Azure
cd publish
func azure functionapp publish $FUNCTION_APP
```

## Post-Deployment Configuration

### 1. Get Function App URL
```bash
az functionapp show \
  --name $FUNCTION_APP \
  --resource-group $RESOURCE_GROUP \
  --query defaultHostName -o tsv
```

Your API will be available at: `https://<function-app-name>.azurewebsites.net`

### 2. Test API Endpoints

**Test with curl:**
```bash
FUNCTION_URL="https://func-nutrition-tracker-dev.azurewebsites.net"

# Get all users
curl "$FUNCTION_URL/api/users"

# Get food nutrition data
curl "$FUNCTION_URL/api/foodnutrition"

# Create a user
curl -X POST "$FUNCTION_URL/api/users" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "John Doe",
    "email": "john@example.com",
    "password": "password123",
    "suggestedCalories": 2000,
    "suggestedCarbs": 250,
    "suggestedFat": 65,
    "suggestedProtein": 150
  }'
```

### 3. Update Frontend Configuration

Update Vue.js API base URL:

**src/services/api.js:**
```javascript
const API_BASE_URL = 'https://func-nutrition-tracker-dev.azurewebsites.net/api';
```

### 4. Monitor Application

**View logs:**
```bash
# Stream logs
az webapp log tail \
  --name $FUNCTION_APP \
  --resource-group $RESOURCE_GROUP

# Or use Azure Portal:
# Function App → Monitoring → Log stream
```

**Application Insights (optional):**
```bash
# Create Application Insights
az monitor app-insights component create \
  --app nutrition-tracker-insights \
  --location $LOCATION \
  --resource-group $RESOURCE_GROUP \
  --application-type web

# Link to Function App
APPINSIGHTS_KEY=$(az monitor app-insights component show \
  --app nutrition-tracker-insights \
  --resource-group $RESOURCE_GROUP \
  --query instrumentationKey -o tsv)

az functionapp config appsettings set \
  --name $FUNCTION_APP \
  --resource-group $RESOURCE_GROUP \
  --settings "APPINSIGHTS_INSTRUMENTATIONKEY=$APPINSIGHTS_KEY"
```

## Cost Estimation

### Free Tier (First 12 months)
- **Azure Functions**: 1M requests/month FREE forever
- **Azure Table Storage**: 5GB storage + 20K operations FREE first year
- **Estimated cost**: $0-0.05/month

### After Free Tier
- **Azure Functions**: $0.20 per million executions
- **Azure Table Storage**: $0.045 per GB/month + $0.00036 per 10K operations
- **Estimated cost**: $0.50-2.00/month (based on moderate usage)

## Troubleshooting

### Functions not appearing
```bash
# Check function app status
az functionapp show --name $FUNCTION_APP --resource-group $RESOURCE_GROUP

# Restart function app
az functionapp restart --name $FUNCTION_APP --resource-group $RESOURCE_GROUP
```

### Connection string issues
```bash
# Verify app settings
az functionapp config appsettings list \
  --name $FUNCTION_APP \
  --resource-group $RESOURCE_GROUP
```

### CORS errors
```bash
# Add specific origin
az functionapp cors add \
  --name $FUNCTION_APP \
  --resource-group $RESOURCE_GROUP \
  --allowed-origins "https://your-frontend-domain.com"
```

## Local Development with Azurite

For local development, use Azurite (Azure Storage Emulator):

```bash
# Install Azurite
npm install -g azurite

# Start Azurite
azurite --silent --location c:\azurite --debug c:\azurite\debug.log

# In another terminal, run Azure Functions
cd src/Adapters/Input/NutritionTracker.AzureFunctions
func start
```

The functions will use `UseDevelopmentStorage=true` from `local.settings.json`.

## Security Best Practices

1. **Use Managed Identity** for Azure resource access
2. **Enable Application Insights** for monitoring
3. **Configure CORS** properly (don't use `*` in production)
4. **Use Azure Key Vault** for secrets
5. **Enable HTTPS only**
6. **Implement authentication** (Azure AD B2C recommended)

## CI/CD Pipeline Overview

### Build and Test Pipeline
- Triggers on push to main/develop branches
- Runs unit tests
- Publishes build artifacts

### Infrastructure Pipeline
- Manual trigger only
- Creates all Azure resources
- Outputs deployment information

### Deploy Pipeline
- Triggers on push to main (after successful build)
- Deploys Azure Functions to Azure
- Validates deployment

## Next Steps

1. ✅ Run Infrastructure pipeline to create Azure resources
2. ✅ Configure GitHub secrets
3. ✅ Run Deploy pipeline to deploy application
4. ✅ Test API endpoints
5. ✅ Update frontend configuration
6. ⏭️ Setup custom domain (optional)
7. ⏭️ Configure Application Insights
8. ⏭️ Implement authentication
