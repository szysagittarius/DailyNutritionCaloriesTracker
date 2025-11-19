# Deployment Structure

This folder contains all deployment-related files for the Nutrition Tracker application.

## Folder Structure

```
deployment/
├── pipelines/              # CI/CD pipeline definitions
│   ├── build-and-test.yml            # GitHub Actions - Build and test
│   ├── infrastructure.yml             # GitHub Actions - Infrastructure deployment
│   ├── deploy-azure-functions.yml    # GitHub Actions - Application deployment
│   ├── azure-devops-build.yml        # Azure DevOps - Build and test
│   ├── azure-devops-infrastructure.yml # Azure DevOps - Infrastructure
│   └── azure-devops-deploy.yml       # Azure DevOps - Application deployment
├── scripts/                # Helper scripts
│   ├── create-service-principal.sh   # Create Azure service principal
│   ├── deploy-infrastructure.sh      # Deploy Azure resources
│   └── deploy-application.sh         # Deploy application code
└── templates/              # Reusable templates (future use)
```

## Platform Support

### GitHub Actions
Pipeline files in `pipelines/` with names starting without prefix are for GitHub Actions.

**Setup:**
1. Pipelines are automatically discovered from `deployment/pipelines/`
2. GitHub Actions workflows in `.github/workflows/` reference these pipelines
3. Configure secrets in GitHub Repository Settings

**Required Secrets:**
- `AZURE_CREDENTIALS` - Service principal JSON
- `AZURE_FUNCTIONAPP_PUBLISH_PROFILE` - Function App publish profile
- `AZURE_RESOURCE_GROUP` - Resource group name

### Azure DevOps
Pipeline files with `azure-devops-` prefix are for Azure DevOps Pipelines.

**Setup:**
1. Create new pipeline in Azure DevOps
2. Select "Existing Azure Pipelines YAML file"
3. Choose file from `deployment/pipelines/azure-devops-*.yml`
4. Configure service connection

**Required Variables:**
- `azureServiceConnection` - Azure service connection name

## Usage

### GitHub Actions
```bash
# Triggered automatically on push to main
# Or run manually from Actions tab
```

### Azure DevOps
```bash
# Configure pipeline in Azure DevOps UI
# Point to: deployment/pipelines/azure-devops-build.yml
```

### Manual Deployment
```bash
# Create service principal
cd deployment/scripts
./create-service-principal.sh

# Deploy infrastructure
./deploy-infrastructure.sh dev eastus

# Deploy application
./deploy-application.sh dev
```

## Pipeline Workflows

### Build and Test
1. Restore dependencies
2. Build solution
3. Run unit tests
4. Publish test results
5. Create deployment artifacts

### Infrastructure Deployment
1. Create Azure Resource Group
2. Create Azure Storage Account (Table Storage)
3. Create Azure Function App (Consumption Plan)
4. Configure connection strings
5. Enable CORS

### Application Deployment
1. Download build artifacts
2. Deploy to Azure Functions
3. Validate deployment
4. Run smoke tests

## Environment Configuration

### Development (dev)
- Resource Group: `rg-nutrition-tracker-dev`
- Function App: `func-nutrition-tracker-dev`
- Storage: `stnutritiontrackerdev`
- Location: East US

### Staging (staging)
- Resource Group: `rg-nutrition-tracker-staging`
- Function App: `func-nutrition-tracker-staging`
- Storage: `stnutritiontrackerstaging`
- Location: East US

### Production (production)
- Resource Group: `rg-nutrition-tracker-production`
- Function App: `func-nutrition-tracker-production`
- Storage: `stnutritiontrackerprod`
- Location: East US

## Cost Optimization

All pipelines use Azure Consumption Plan:
- **First 1M requests/month**: FREE
- **Beyond free tier**: $0.20 per million executions
- **Expected cost**: $0-2/month

## Documentation

- [Deployment Guide](../docs/DEPLOYMENT.md) - Complete deployment instructions
- [Azure Setup](../docs/AZURE_SETUP.md) - Azure configuration guide
- [Architecture](../docs/Hexagonal%20Architecture%20Implementation.md) - System architecture
