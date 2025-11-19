# Azure Setup Instructions

## Quick Start - Create Service Principal for GitHub Actions

### Step 1: Create Service Principal

```bash
# Login to Azure
az login

# Set your subscription (if you have multiple)
az account list --output table
az account set --subscription "YOUR_SUBSCRIPTION_NAME_OR_ID"

# Get subscription ID
SUBSCRIPTION_ID=$(az account show --query id -o tsv)

# Create service principal with Contributor role
az ad sp create-for-rbac \
  --name "github-nutrition-tracker-deploy" \
  --role Contributor \
  --scopes /subscriptions/$SUBSCRIPTION_ID \
  --sdk-auth

# This outputs JSON - copy the entire output for AZURE_CREDENTIALS secret
```

**Copy the JSON output and save it as GitHub Secret `AZURE_CREDENTIALS`**

### Step 2: Add GitHub Secrets

Go to: **GitHub Repository → Settings → Secrets and variables → Actions → New repository secret**

Add these secrets:

1. **AZURE_CREDENTIALS**: (JSON from above)
2. **AZURE_RESOURCE_GROUP**: `rg-nutrition-tracker-dev` (or your preferred name)

### Step 3: Run Infrastructure Pipeline

1. Go to **Actions** tab
2. Select **"Deploy Azure Infrastructure"** workflow
3. Click **"Run workflow"**
4. Select environment: `dev`
5. Click **"Run workflow"**

Wait for completion, then download the publish profile artifact.

### Step 4: Add Publish Profile Secret

1. Download `publish-profile.xml` from workflow artifacts
2. Open the file and copy entire contents
3. Add as GitHub Secret: **AZURE_FUNCTIONAPP_PUBLISH_PROFILE**

### Step 5: Deploy Application

1. Push to `main` branch, OR
2. Run **"Deploy Azure Functions"** workflow manually

Your API is now live! 🎉

## Resource Naming Convention

| Resource Type | Name Format | Example |
|--------------|-------------|---------|
| Resource Group | `rg-nutrition-tracker-{env}` | `rg-nutrition-tracker-dev` |
| Storage Account | `stnutritiontracker{env}` | `stnutritiontrackerdev` |
| Function Storage | `stfuncnutrition{env}` | `stfuncnutritiondev` |
| Function App | `func-nutrition-tracker-{env}` | `func-nutrition-tracker-dev` |

## Cost Breakdown

### Consumption Plan (Recommended)

**Azure Functions (Consumption Plan Y1):**
- First 1,000,000 executions: **FREE**
- First 400,000 GB-seconds: **FREE**
- Beyond free tier: $0.20 per million executions

**Azure Table Storage:**
- First year (12 months): **FREE** (5GB + 20K operations)
- Storage: $0.045 per GB/month
- Operations: $0.00036 per 10K operations

**Expected Monthly Cost:**
- First year: **$0.00 - $0.05**
- After first year: **$0.50 - $2.00**
- With moderate usage (10K requests/month): **~$0.05**

### Premium Plan (If needed for performance)

If you need guaranteed performance:

```bash
# Create App Service Plan (Premium P1v2)
az functionapp plan create \
  --name asp-nutrition-tracker-dev \
  --resource-group rg-nutrition-tracker-dev \
  --location eastus \
  --sku P1v2 \
  --is-linux

# Create Function App on Premium Plan
az functionapp create \
  --name func-nutrition-tracker-dev \
  --resource-group rg-nutrition-tracker-dev \
  --plan asp-nutrition-tracker-dev \
  --storage-account stfuncnutritiondev \
  --runtime dotnet-isolated \
  --runtime-version 8 \
  --functions-version 4
```

**Premium P1v2 Cost:** ~$75/month (not recommended unless needed)

## Verify Deployment

### Check Resources
```bash
# List all resources in resource group
az resource list --resource-group rg-nutrition-tracker-dev --output table

# Check Function App status
az functionapp show \
  --name func-nutrition-tracker-dev \
  --resource-group rg-nutrition-tracker-dev \
  --query "{Name:name, Status:state, URL:defaultHostName}" -o table

# Get Function App URL
az functionapp show \
  --name func-nutrition-tracker-dev \
  --resource-group rg-nutrition-tracker-dev \
  --query defaultHostName -o tsv
```

### Test Endpoints
```bash
FUNCTION_URL="https://$(az functionapp show --name func-nutrition-tracker-dev --resource-group rg-nutrition-tracker-dev --query defaultHostName -o tsv)"

echo "Testing: $FUNCTION_URL"

# Test health/functions list
curl "$FUNCTION_URL/api/users" -v

# Should return empty array or data
```

## Table Storage Setup

Tables are created automatically by the application on first use:
- `Users`
- `FoodLogs`
- `FoodNutrition`

### Manually verify tables (optional)
```bash
# Get connection string
CONNECTION_STRING=$(az storage account show-connection-string \
  --name stnutritiontrackerdev \
  --resource-group rg-nutrition-tracker-dev \
  --query connectionString -o tsv)

# List tables
az storage table list --connection-string "$CONNECTION_STRING" --output table
```

## Monitoring

### Enable Application Insights
```bash
# Create Application Insights
az monitor app-insights component create \
  --app nutrition-tracker-insights-dev \
  --location eastus \
  --resource-group rg-nutrition-tracker-dev \
  --application-type web

# Get instrumentation key
APPINSIGHTS_KEY=$(az monitor app-insights component show \
  --app nutrition-tracker-insights-dev \
  --resource-group rg-nutrition-tracker-dev \
  --query instrumentationKey -o tsv)

# Add to Function App
az functionapp config appsettings set \
  --name func-nutrition-tracker-dev \
  --resource-group rg-nutrition-tracker-dev \
  --settings "APPINSIGHTS_INSTRUMENTATIONKEY=$APPINSIGHTS_KEY"
```

### View Logs
```bash
# Stream logs in real-time
az webapp log tail \
  --name func-nutrition-tracker-dev \
  --resource-group rg-nutrition-tracker-dev
```

## Cleanup

To delete all resources:
```bash
# Delete entire resource group
az group delete --name rg-nutrition-tracker-dev --yes --no-wait
```

## Troubleshooting

### Issue: Function App not starting
```bash
# Check logs
az webapp log tail --name func-nutrition-tracker-dev --resource-group rg-nutrition-tracker-dev

# Restart
az functionapp restart --name func-nutrition-tracker-dev --resource-group rg-nutrition-tracker-dev
```

### Issue: CORS errors from frontend
```bash
# Add your frontend domain
az functionapp cors add \
  --name func-nutrition-tracker-dev \
  --resource-group rg-nutrition-tracker-dev \
  --allowed-origins "https://your-frontend.com"

# For development, allow all (not for production!)
az functionapp cors add \
  --name func-nutrition-tracker-dev \
  --resource-group rg-nutrition-tracker-dev \
  --allowed-origins "*"
```

### Issue: Connection string not working
```bash
# Verify app settings
az functionapp config appsettings list \
  --name func-nutrition-tracker-dev \
  --resource-group rg-nutrition-tracker-dev \
  --query "[?name=='AzureTableStorageConnectionString']"
```

## Next Steps

1. ✅ Complete Azure setup
2. ✅ Deploy application via GitHub Actions
3. ⏭️ Update Vue.js frontend to use Azure Functions URL
4. ⏭️ Test all endpoints
5. ⏭️ Setup custom domain (optional)
6. ⏭️ Implement authentication (Azure AD B2C)
7. ⏭️ Setup monitoring and alerts
8. ⏭️ Create production environment
