# Deployment Structure

This folder contains all deployment-related files for the Nutrition Tracker application.

## Folder Structure

```
.github/workflows/           # The ACTUAL GitHub Actions workflows (self-contained)
│   ├── build.yml                     # CI - build + test, on push/PR to main
│   ├── deploy-backend.yml            # CD - deploy Azure Functions, on push to main
│   ├── deploy-frontend.yml           # CD - deploy Static Web App, on push to main
│   └── infrastructure.yml            # CD - create/update Azure resources
deployment/
├── pipelines/              # Azure DevOps pipeline definitions (portable reference)
│   ├── azure-devops-build.yml        # Azure DevOps - Build and test
│   ├── azure-devops-infrastructure.yml # Azure DevOps - Infrastructure
│   └── azure-devops-deploy.yml       # Azure DevOps - Application deployment
├── scripts/                # Helper scripts for manual/local deployment
│   ├── create-service-principal.sh   # Create Azure service principal
│   ├── deploy-infrastructure.sh      # Deploy Azure resources (incl. Static Web App)
│   └── deploy-application.sh         # Deploy application code
└── templates/              # Reusable templates (future use)
```

> **Note**: GitHub requires reusable/callable workflow files to live directly under `.github/workflows/`
> (subdirectories are not supported), so the real CI/CD logic lives there now instead of being split
> into a separate `deployment/pipelines/*.yml` + wrapper indirection.

## Platform Support

### GitHub Actions
The real, runnable workflows are the files in `.github/workflows/`. They trigger automatically on every
push to `main` (see [GITHUB_ACTIONS_SAFETY.md](GITHUB_ACTIONS_SAFETY.md) for the exact trigger rules).

**Setup:**
1. Configure the secrets/variables listed below in GitHub Repository Settings
2. Push to `main` — build, backend deploy, and frontend deploy run automatically
3. Infrastructure only re-runs automatically when infra scripts change; otherwise run it manually once via `workflow_dispatch`

**Required Secrets:**
- `AZURE_CREDENTIALS` - Service principal JSON (used by `infrastructure.yml`)
- `AZURE_FUNCTIONAPP_PUBLISH_PROFILE` - Function App publish profile (used by `deploy-backend.yml`)
- `AZURE_STATIC_WEB_APPS_API_TOKEN` - Static Web App deployment token (used by `deploy-frontend.yml`)

**Required Repository Variable:**
- `FUNCTION_APP_URL` - e.g. `https://func-nutrition-tracker-dev.azurewebsites.net` (baked into the frontend build as `VITE_API_URL`)

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
4. Create Azure Static Web App (Free tier, frontend hosting)
5. Configure connection strings
6. Enable CORS

### Backend Deployment
1. Build + test the solution
2. Publish the Azure Functions project
3. Deploy to the Function App via publish profile

### Frontend Deployment
1. `npm ci` + `npm run build` (Vite build of the Vue app)
2. Deploy the `dist/` output to the Static Web App
3. PR pushes get their own preview environment; closing the PR tears it down

## Environment Configuration

### Development (dev) — the environment auto-deployed on every push to `main`
- Resource Group: `rg-nutrition-tracker-dev`
- Function App: `func-nutrition-tracker-dev`
- Storage: `stnutritiontrackerdev`
- Static Web App: `swa-nutrition-tracker-dev`
- Location: East US (Static Web App: East US 2 — Free tier region restriction)

### Staging (staging) / Production (production)
Same naming pattern with `-staging` / `-production` suffixes. These are only created when you
run the **Deploy Infrastructure** workflow manually with that environment selected; the automatic
push-triggered deploys always target `dev`.

## Cost Optimization (Free tier)

- **Azure Functions (Consumption Y1)**: first 1M executions + 400,000 GB-s/month FREE
- **Azure Table Storage**: pennies/month at low volume (pay-as-you-go, no free-tier SKU, but very cheap)
- **Azure Static Web Apps (Free SKU)**: 100 GB bandwidth/month, free SSL/custom domain, FREE forever
- **Expected cost**: ~$0.05–$2.00/month

See [Docs/CICD-Pipeline-And-Hosting-Plan.md](../Docs/CICD-Pipeline-And-Hosting-Plan.md) for the full plan.

## Documentation

- [Deployment Guide](DEPLOYMENT.md) - Complete deployment instructions
- [Azure Setup](AZURE_SETUP.md) - Azure configuration guide
- [GitHub Actions Safety](GITHUB_ACTIONS_SAFETY.md) - Trigger behavior and secrets checklist
- [CI/CD Pipeline & Hosting Plan](../Docs/CICD-Pipeline-And-Hosting-Plan.md) - End-to-end plan and setup checklist
- [Architecture](../Docs/Hexagonal%20Architecture%20Implementation.md) - System architecture
