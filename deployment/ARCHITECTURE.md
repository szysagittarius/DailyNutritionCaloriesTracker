# Deployment Architecture Guide

> **Update**: This repo now follows **Option A** below — GitHub Actions workflows are self-contained
> files directly under `.github/workflows/`. GitHub does **not** allow reusable/callable workflow
> files outside `.github/workflows/` (subdirectories aren't supported either), so the previous
> design that tried to "call" YAML files living in `deployment/pipelines/` could never actually run.
> `deployment/pipelines/azure-devops-*.yml` remain as the Azure DevOps equivalents (Azure DevOps has
> no such restriction).

## Why This Structure?

This deployment structure is designed to be **simple, flexible, and platform-agnostic**. Here's why each piece exists:

---

## CI/CD Components Explained

### What is CI/CD?

**CI (Continuous Integration)**: Automatically build and test code when changes are pushed
- ✅ Build the application
- ✅ Run unit tests
- ✅ Check code quality
- ✅ Create artifacts (compiled code)

**CD (Continuous Deployment)**: Automatically deploy code to servers/cloud
- ✅ Deploy to Azure
- ✅ Create infrastructure
- ✅ Update configuration
- ✅ Verify deployment

### CI/CD in Our Project

| File | Type | Purpose | Trigger |
|------|------|---------|---------|
| **CI** ||||
| `.github/workflows/build.yml` | **CI** | Build + Test | Push/PR to `main` |
| `azure-devops-build.yml` | **CI** | Build + Test | Push/PR to `main`/`develop` |
| **CD** ||||
| `.github/workflows/infrastructure.yml` | **CD** | Create/update Azure resources (Functions + Table Storage + Static Web App) | Manual, or push when infra scripts change |
| `.github/workflows/deploy-backend.yml` | **CD** | Deploy Azure Functions API | Push to `main` (path-filtered) |
| `.github/workflows/deploy-frontend.yml` | **CD** | Deploy Vue app to Static Web App | Push to `main` (path-filtered) |
| `azure-devops-infrastructure.yml` | **CD** | Create Azure resources | Manual trigger only |
| `azure-devops-deploy.yml` | **CD** | Deploy application code | Push to main (after CI) |
| **MANUAL SCRIPTS** ||||
| `create-service-principal.sh` | Setup | One-time Azure setup | Manual |
| `deploy-infrastructure.sh` | **CD** | Create Azure resources | Manual |
| `deploy-application.sh` | **CD** | Deploy application | Manual |

### CI Pipeline Flow

```
Developer pushes code to GitHub/Azure DevOps
         ↓
   [CI PIPELINE STARTS]
         ↓
1. Checkout code
2. Restore dependencies (dotnet restore)
3. Build solution (dotnet build)
4. Run tests (dotnet test)
5. Publish artifacts (dotnet publish)
         ↓
   [CI PIPELINE ENDS]
         ↓
   Build artifacts stored
   (ready for deployment)
```

**Files involved**:
- `.github/workflows/build.yml` (full CI logic, self-contained)
- `deployment/pipelines/azure-devops-build.yml` (for Azure DevOps)

### CD Pipeline Flow

```
   CI Pipeline completes successfully
         ↓
   [CD PIPELINE STARTS]
         ↓
1. Download build artifacts
2. Login to Azure
3. Deploy to Azure Functions
4. Update app settings
5. Restart function app
6. Verify deployment
         ↓
   [CD PIPELINE ENDS]
         ↓
   Application live on Azure!
```

**Files involved**:
- `.github/workflows/deploy-backend.yml` (Azure Functions, full CD logic, self-contained)
- `.github/workflows/deploy-frontend.yml` (Static Web App, full CD logic, self-contained)
- `deployment/pipelines/azure-devops-deploy.yml` (for Azure DevOps)

### Infrastructure Pipeline (Special CD)

This is a **one-time CD pipeline** that creates Azure resources:

```
   Manual trigger (first time only)
         ↓
   [INFRASTRUCTURE PIPELINE]
         ↓
1. Create Resource Group
2. Create Storage Account (for Table Storage)
3. Create Function App (serverless)
4. Configure connection strings
5. Enable CORS
         ↓
   Azure resources ready!
         ↓
   Now you can run deploy pipeline
```

**Files involved**:
- `.github/workflows/infrastructure.yml` (full infra logic, self-contained; adds Static Web App)
- `deployment/pipelines/azure-devops-infrastructure.yml` (for Azure DevOps)

---

### 📁 Folder Structure

```
.github/workflows/          ← The real, self-contained GitHub Actions workflows
deployment/
  ├── pipelines/           ← Azure DevOps pipeline definitions (portable reference)
  └── scripts/             ← Reusable bash scripts for manual deployment
```

## The Layers Explained

### 1. `.github/workflows/` - GitHub Actions Workflows (Self-Contained)

**Purpose**: Contains the actual CI/CD logic that GitHub Actions runs.

**Type**: CI/CD Implementation

**⚠️ IMPORTANT: Auto-Triggers When Pushed to GitHub!**

When you commit and push files to `.github/workflows/` folder:
- ✅ GitHub automatically detects YAML files in `.github/workflows/`
- ✅ Workflows become active immediately (no manual setup needed)
- ✅ They will start running based on their triggers:
  - `on: push` → Runs when you push code
  - `on: pull_request` → Runs when you create a PR
  - `on: workflow_dispatch` → Only runs when manually triggered

**Current trigger behavior in this repo** (see [GITHUB_ACTIONS_SAFETY.md](GITHUB_ACTIONS_SAFETY.md) for full detail):

| Workflow | Auto-trigger |
|----------|--------------|
| `build.yml` | Push/PR to `main` |
| `deploy-backend.yml` | Push to `main`, only when backend/Core paths change |
| `deploy-frontend.yml` | Push to `main`, only when frontend paths change; PRs get preview deploys |
| `infrastructure.yml` | Manual by default; auto-runs only if the infra script/workflow itself changes |

**Why GitHub requires this**:
- GitHub Actions requires runnable/callable workflow YAML files to live directly in `.github/workflows/`
  (subdirectories, e.g. `deployment/pipelines/`, are **not** supported for `uses:` references)
- So, unlike Azure DevOps (which supports pipeline files anywhere in the repo), the real logic has to
  live here rather than being split into a separate folder with a thin trigger wrapper

**Can I skip this?**
- ✅ YES if you're using Azure DevOps instead (use `deployment/pipelines/azure-devops-*.yml`)
- ✅ YES if you prefer manual deployment via `deployment/scripts/`
- ❌ NO if you want automated GitHub Actions deployments

---

### 2. `deployment/pipelines/azure-devops-*.yml` - Azure DevOps Pipelines

**Purpose**: Equivalent CI/CD logic for teams using Azure DevOps instead of GitHub Actions.

**Type**: CI + CD Implementation (Azure DevOps flavor)

**Why it exists**:
- Azure DevOps pipeline files can live anywhere in the repo and are referenced by path in the Azure DevOps UI
- Kept in sync with the GitHub Actions workflows so either platform can be used

**What's inside**:
```yaml
# deployment/pipelines/azure-devops-build.yml (CI)
steps:
  - Restore dependencies (dotnet restore)
  - Build solution (dotnet build)
  - Run tests (dotnet test)
  - Publish artifacts (dotnet publish)

# deployment/pipelines/azure-devops-deploy.yml (CD)
steps:
  - Download build artifacts
  - Login to Azure
  - Deploy to Azure Functions
  - Configure app settings

# deployment/pipelines/azure-devops-infrastructure.yml (CD - One-time / on-demand)
steps:
  - Create Resource Group
  - Create Storage Account
  - Create Function App
  - Create Static Web App (Free tier)
  - Configure connection strings
```

**Can I skip this?**
- ❌ NO if you want automated CI/CD
- ✅ YES if you only use manual scripts (but not recommended)

**Platform Support**:
| File | Platform | Type | Purpose |
|------|----------|------|---------|
| `build-and-test.yml` | GitHub Actions | **CI** | Build and test code |
| `infrastructure.yml` | GitHub Actions | **CD** | Create Azure resources |
| `deploy-azure-functions.yml` | GitHub Actions | **CD** | Deploy application to Azure |
| `azure-devops-build.yml` | Azure DevOps | **CI** | Build and test code |
| `azure-devops-infrastructure.yml` | Azure DevOps | **CD** | Create Azure resources |
| `azure-devops-deploy.yml` | Azure DevOps | **CD** | Deploy application to Azure |

---

### 3. `deployment/scripts/` - Bash Scripts (Alternative)

**Purpose**: Manual deployment scripts for developers who want direct control.

**Type**: Manual CD (no automation)

**Why it exists**:
- Quick local deployment without CI/CD setup
- Testing deployments before committing pipelines
- Emergency deployments when CI/CD is down
- Learning and debugging
- Alternative to automated CD pipelines

**What's inside**:
```bash
# deployment/scripts/create-service-principal.sh (Setup - One-time)
az ad sp create-for-rbac --name "github-deploy" --role Contributor

# deployment/scripts/deploy-infrastructure.sh (CD - Manual)
az group create --name rg-nutrition-tracker-dev
az storage account create --name stnutritiontrackerdev
az functionapp create --name func-nutrition-tracker-dev
# Creates all Azure resources

# deployment/scripts/deploy-application.sh (CD - Manual)
dotnet publish --configuration Release
func azure functionapp publish func-nutrition-tracker-dev
# Deploys your code to Azure
```

**CI equivalent for scripts?**
- ❌ NO - Scripts skip CI phase (no automated testing)
- ⚠️ You should manually run `dotnet build` and `dotnet test` before deploying
- Scripts are pure CD (deployment only)

**Can I skip this?**
- ✅ YES if you only use CI/CD pipelines
- ❌ NO if you want the option to deploy manually

---

## Simplified Decision Tree

### "Which deployment method should I use?"

```
Do you have a CI/CD system?
│
├─ YES, I use GitHub Actions (this repo's default — already set up)
│  ├─ Keep: .github/workflows/ (self-contained, real workflows)
│  └─ Optional: deployment/scripts/ (for manual testing)
│
├─ YES, I use Azure DevOps
│  ├─ Keep: deployment/pipelines/azure-devops-*.yml
│  ├─ Remove: .github/workflows/ (not needed)
│  └─ Optional: deployment/scripts/ (for manual testing)
│
└─ NO, I deploy manually
   ├─ Keep: deployment/scripts/
   └─ Remove: .github/workflows/ + deployment/pipelines/
```

This repo currently keeps all three options available at once, so you can switch platforms without
restructuring anything.

---

## Quick Start Guide

### For GitHub Actions Users

1. **One-time setup**:
   ```bash
   # Create Azure service principal
   cd deployment/scripts
   ./create-service-principal.sh
   
   # Add output as GitHub Secret: AZURE_CREDENTIALS
   ```

2. **Deploy**:
   - Push to `main` branch
   - Or click "Run workflow" in GitHub Actions tab
   - Done! ✅

### For Azure DevOps Users

1. **One-time setup**:
   - Create pipeline pointing to `deployment/pipelines/azure-devops-build.yml`
   - Create service connection to Azure
   - Set variable: `azureServiceConnection`

2. **Deploy**:
   - Push to `main` branch
   - Or trigger pipeline manually
   - Done! ✅

### For Manual Deployment Users

```bash
# One-time: Create resources
cd deployment/scripts
./deploy-infrastructure.sh dev eastus

# Every deployment: Deploy code
./deploy-application.sh dev
```

---

## Real-World Example

### Current Structure (Option A: self-contained GitHub Actions workflows)

```
DailyNutritionCaloriesTracker/
├── .github/workflows/            # The real, runnable GitHub Actions workflows
│   ├── build.yml                    # CI - build + test
│   ├── deploy-backend.yml           # CD - deploy Azure Functions
│   ├── deploy-frontend.yml          # CD - deploy Static Web App
│   └── infrastructure.yml           # CD - create/update Azure resources
│
├── deployment/
│   ├── pipelines/                   # Azure DevOps equivalents (portable reference)
│   │   ├── azure-devops-build.yml
│   │   ├── azure-devops-infrastructure.yml
│   │   └── azure-devops-deploy.yml
│   │
│   ├── scripts/                     # Manual deployment
│   │   ├── create-service-principal.sh
│   │   ├── deploy-infrastructure.sh
│   │   └── deploy-application.sh
│   │
│   └── README.md
│
└── src/                          # Application code (frontend + backend)
```

**Pros**:
- ✅ Works with GitHub Actions out of the box (no broken indirection)
- ✅ Azure DevOps equivalents kept for teams that use it instead
- ✅ Manual scripts available as a fallback
- ✅ Simple: one place to look for the real GitHub Actions logic

---

## Recommendation

### For This Project: Option A is already in place

**Why?**
1. GitHub Actions is the repo's primary CI/CD platform, and reusable workflow files **must** live in
   `.github/workflows/` — so the previous "thin trigger + external pipeline" split could never work
2. Keeping the Azure DevOps YAML files in `deployment/pipelines/` still gives you a migration path
   without any restructuring, since that platform has no such restriction
3. `deployment/scripts/` remains available for manual/emergency deployment

No further restructuring is needed — this is the simplest structure that still works correctly on GitHub.

---

## Summary: CI vs CD Breakdown

### CI (Continuous Integration) - Build & Test
| Component | CI/CD | Purpose |
|-----------|-------|---------|
| `.github/workflows/build.yml` | **CI** | Builds code, runs tests |
| `deployment/pipelines/azure-devops-build.yml` | **CI** | Builds code, runs tests (Azure DevOps) |

**When it runs**: Every push to `main`, every pull request

**What it does**:
1. ✅ Restore dependencies
2. ✅ Build solution
3. ✅ Run unit tests
4. ✅ Create build artifacts

---

### CD (Continuous Deployment) - Deploy to Azure
| Component | CI/CD | Purpose |
|-----------|-------|---------|
| `.github/workflows/infrastructure.yml` | **CD** | Creates/updates Azure resources |
| `.github/workflows/deploy-backend.yml` | **CD** | Deploys Azure Functions API |
| `.github/workflows/deploy-frontend.yml` | **CD** | Deploys Vue app to Static Web App |
| `deployment/pipelines/azure-devops-*.yml` | **CD** | Azure DevOps versions |
| `deployment/scripts/*.sh` | **CD Scripts** | Manual deployment |

**When it runs**:
- Infrastructure: Manual trigger, or automatically when the infra script/workflow itself changes
- Backend/Frontend deploy: On push to `main` (path-filtered to relevant folders)

**What it does**:
1. ✅ Create/update Azure resources
2. ✅ Deploy application code
3. ✅ Configure settings
4. ✅ Verify deployment

---

### Quick Reference

| Component | Type | Required? | Purpose |
|-----------|------|-----------|---------|
| `.github/workflows/` | CI/CD Implementation | ✅ For GitHub Actions | The ACTUAL CI/CD logic |
| `deployment/pipelines/azure-devops-*.yml` | CI/CD Logic | ✅ For Azure DevOps | Azure DevOps equivalent |
| `deployment/scripts/` | Manual CD | ⚠️ Optional | Alternative to automated CD |

---

### Full CI/CD Flow (GitHub Actions)

```
Developer pushes code to main branch
           ↓
┌──────────────────────────────────┐
│  CI PHASE (Continuous Integration) │
└──────────────────────────────────┘
           ↓
[Run] .github/workflows/build.yml
           ↓
    1. Restore dependencies
    2. Build solution
    3. Run tests
    4. Create artifacts
           ↓
       CI Succeeds ✅
           ↓
┌──────────────────────────────────┐
│  CD PHASE (Continuous Deployment)  │
└──────────────────────────────────┘
           ↓
[Run] .github/workflows/deploy-backend.yml + deploy-frontend.yml (in parallel)
           ↓
    1. Build & publish Azure Functions / Vue app
    2. Deploy to Function App / Static Web App
    3. Configure app settings
    4. Verify deployment
           ↓
       CD Succeeds ✅
           ↓
    Application live on Azure! 🚀
```

---

**Recommended**: Keep the current structure — self-contained GitHub Actions workflows plus Azure DevOps
equivalents and manual scripts as fallbacks. No further simplification is needed.
