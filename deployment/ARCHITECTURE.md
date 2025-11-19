# Deployment Architecture Guide

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
| **CI PIPELINES** ||||
| `build-and-test.yml` | **CI** | Build + Test | Every push to main/develop |
| `azure-devops-build.yml` | **CI** | Build + Test | Every push to main/develop |
| **CD PIPELINES** ||||
| `infrastructure.yml` | **CD** | Create Azure resources | Manual trigger only |
| `deploy-azure-functions.yml` | **CD** | Deploy application code | Push to main (after CI) |
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
- `.github/workflows/build.yml` → triggers → `deployment/pipelines/build-and-test.yml`
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
- `.github/workflows/deploy.yml` → triggers → `deployment/pipelines/deploy-azure-functions.yml`
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
- `.github/workflows/infrastructure.yml` → triggers → `deployment/pipelines/infrastructure.yml`
- `deployment/pipelines/azure-devops-infrastructure.yml` (for Azure DevOps)

---

### 📁 Folder Structure

```
.github/workflows/          ← Minimal GitHub Actions triggers
deployment/
  ├── pipelines/           ← Actual pipeline definitions (platform-agnostic)
  └── scripts/             ← Reusable bash scripts for manual deployment
```

## The Three Layers Explained

### 1. `.github/workflows/` - GitHub Actions Triggers (Optional)

**Purpose**: Lightweight trigger files that tell GitHub Actions WHEN to run pipelines.

**Type**: CI/CD Orchestrator (tells when to run CI or CD)

**⚠️ IMPORTANT: Will Auto-Trigger When Pushed to GitHub!**

When you commit and push files to `.github/workflows/` folder:
- ✅ GitHub automatically detects YAML files in `.github/workflows/`
- ✅ Workflows become active immediately (no manual setup needed)
- ✅ They will start running based on their triggers:
  - `on: push` → Runs when you push code
  - `on: pull_request` → Runs when you create a PR
  - `on: workflow_dispatch` → Only runs when manually triggered

**Example Behavior**:
```yaml
# .github/workflows/build.yml
on:
  push:
    branches: [main]  # ⚠️ Will run EVERY TIME you push to main!
```

**To Prevent Unwanted Triggers**:
1. Use `workflow_dispatch` (manual trigger only):
   ```yaml
   on:
     workflow_dispatch:  # ✅ Safe - only runs when you click "Run workflow"
   ```

2. Add path filters to limit when it runs:
   ```yaml
   on:
     push:
       paths:
         - 'src/**'  # ✅ Only runs when code in src/ changes
   ```

3. Disable workflow temporarily:
   - Rename file: `build.yml` → `build.yml.disabled`
   - Or add to `.gitignore` until ready

**Why it exists**: 
- GitHub Actions requires YAML files in `.github/workflows/` to detect workflows
- These are just "entry points" - they don't contain the actual deployment logic
- They trigger the actual CI/CD pipelines in `deployment/pipelines/`

**What's inside**:
```yaml
# .github/workflows/build.yml (CI TRIGGER)
on:
  push:
    branches: [main, develop]
  pull_request:
    branches: [main, develop]
jobs:
  build:
    uses: deployment/pipelines/build-and-test.yml  # Runs CI

# .github/workflows/deploy.yml (CD TRIGGER)
on:
  push:
    branches: [main]
jobs:
  deploy:
    uses: deployment/pipelines/deploy-azure-functions.yml  # Runs CD

# .github/workflows/infrastructure.yml (CD TRIGGER - Manual)
on:
  workflow_dispatch:  # Manual trigger only
jobs:
  deploy:
    uses: deployment/pipelines/infrastructure.yml  # Runs CD
```

**Can I skip this?** 
- ✅ YES if you're using Azure DevOps
- ✅ YES if you prefer manual deployment via scripts
- ❌ NO if you want automated GitHub Actions deployments

---

### 2. `deployment/pipelines/` - Pipeline Definitions (Core)

**Purpose**: Contains the ACTUAL CI/CD logic - works with ANY CI/CD system.

**Type**: CI + CD Implementation (the actual work happens here)

**Why it exists**:
- Keeps deployment logic separate from application code
- Easy to version control
- Can be used by GitHub Actions, Azure DevOps, GitLab CI, or manually
- Industry standard (most Azure DevOps repos use a `deployment/` or `pipelines/` folder)

**What's inside**:
```yaml
# deployment/pipelines/build-and-test.yml (CI)
steps:
  - Restore dependencies (dotnet restore)
  - Build solution (dotnet build)
  - Run tests (dotnet test)
  - Publish artifacts (dotnet publish)

# deployment/pipelines/deploy-azure-functions.yml (CD)
steps:
  - Download build artifacts
  - Login to Azure
  - Deploy to Azure Functions
  - Configure app settings
  - Verify deployment

# deployment/pipelines/infrastructure.yml (CD - One-time)
steps:
  - Create Resource Group
  - Create Storage Account
  - Create Function App
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
├─ YES, I use GitHub Actions
│  ├─ Keep: .github/workflows/ + deployment/pipelines/
│  └─ Optional: deployment/scripts/ (for manual testing)
│
├─ YES, I use Azure DevOps
│  ├─ Keep: deployment/pipelines/ (azure-devops-*.yml files)
│  ├─ Remove: .github/workflows/ (not needed)
│  └─ Optional: deployment/scripts/ (for manual testing)
│
└─ NO, I deploy manually
   ├─ Keep: deployment/scripts/
   └─ Remove: .github/workflows/ + deployment/pipelines/*.yml
```

---

## Can We Make It Simpler?

### Option A: GitHub Actions Only (Simplest)

If you ONLY use GitHub Actions and never plan to use Azure DevOps:

**Keep**:
```
.github/workflows/
  ├── build.yml
  ├── infrastructure.yml
  └── deploy.yml
deployment/
  └── scripts/           (optional for manual deployment)
```

**Remove**:
```
deployment/pipelines/   (move YAML content into .github/workflows/)
```

### Option B: Azure DevOps Only

If you ONLY use Azure DevOps:

**Keep**:
```
deployment/
  ├── pipelines/
  │   ├── azure-devops-build.yml
  │   ├── azure-devops-infrastructure.yml
  │   └── azure-devops-deploy.yml
  └── scripts/           (optional)
```

**Remove**:
```
.github/workflows/      (not needed)
deployment/pipelines/build-and-test.yml  (GitHub Actions specific)
deployment/pipelines/infrastructure.yml   (GitHub Actions specific)
deployment/pipelines/deploy-azure-functions.yml (GitHub Actions specific)
```

### Option C: Manual Only (Simplest of All)

If you ONLY deploy manually via scripts:

**Keep**:
```
deployment/
  └── scripts/
      ├── create-service-principal.sh
      ├── deploy-infrastructure.sh
      └── deploy-application.sh
```

**Remove**:
```
.github/workflows/      (not needed)
deployment/pipelines/   (not needed)
```

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

### Current Structure (Flexible - Supports All Methods)

```
DailyNutritionCaloriesTracker/
├── .github/workflows/          # GitHub Actions triggers
│   ├── build.yml
│   ├── infrastructure.yml
│   └── deploy.yml
│
├── deployment/
│   ├── pipelines/              # Actual pipeline logic
│   │   ├── build-and-test.yml              # For GitHub
│   │   ├── infrastructure.yml               # For GitHub
│   │   ├── deploy-azure-functions.yml      # For GitHub
│   │   ├── azure-devops-build.yml          # For Azure DevOps
│   │   ├── azure-devops-infrastructure.yml # For Azure DevOps
│   │   └── azure-devops-deploy.yml         # For Azure DevOps
│   │
│   ├── scripts/                # Manual deployment
│   │   ├── create-service-principal.sh
│   │   ├── deploy-infrastructure.sh
│   │   └── deploy-application.sh
│   │
│   └── README.md
│
└── src/                        # Application code
```

**Pros**: 
- ✅ Works with GitHub Actions
- ✅ Works with Azure DevOps
- ✅ Can deploy manually
- ✅ Easy to switch platforms

**Cons**:
- ❌ Slightly more files
- ❌ Need to understand the structure

---

### Simplified Structure (GitHub Actions Only)

```
DailyNutritionCaloriesTracker/
├── .github/workflows/          # Everything in one place
│   ├── build.yml              # Contains build logic
│   ├── infrastructure.yml      # Contains infra logic
│   └── deploy.yml             # Contains deploy logic
│
├── deployment/
│   └── scripts/               # Optional: manual deployment
│       ├── deploy-infrastructure.sh
│       └── deploy-application.sh
│
└── src/                       # Application code
```

**Pros**: 
- ✅ Simpler structure
- ✅ Everything in one place
- ✅ Less files to maintain

**Cons**:
- ❌ Locked into GitHub Actions
- ❌ Harder to migrate to Azure DevOps
- ❌ Mixes platform-specific and deployment logic

---

## Recommendation

### For This Project: Keep Current Structure

**Why?**
1. **Future-proof**: Easy to migrate between GitHub Actions and Azure DevOps
2. **Best practice**: Separates deployment logic from platform-specific triggers
3. **Professional**: Standard structure used by most enterprise projects
4. **Minimal complexity**: Only 3 extra files in `.github/workflows/`

### Want Simpler? Follow Option A (GitHub Actions Only)

1. Move content from `deployment/pipelines/*.yml` into `.github/workflows/`
2. Delete `deployment/pipelines/` folder
3. Keep `deployment/scripts/` for manual deployment option

---

## Summary: CI vs CD Breakdown

### CI (Continuous Integration) - Build & Test
| Component | CI/CD | Purpose |
|-----------|-------|---------|
| `.github/workflows/build.yml` | **CI Trigger** | Tells GitHub Actions when to build |
| `deployment/pipelines/build-and-test.yml` | **CI Pipeline** | Builds code, runs tests |
| `deployment/pipelines/azure-devops-build.yml` | **CI Pipeline** | Builds code, runs tests (Azure DevOps) |

**When it runs**: Every push to main/develop, every pull request

**What it does**:
1. ✅ Restore dependencies
2. ✅ Build solution
3. ✅ Run unit tests
4. ✅ Create build artifacts

---

### CD (Continuous Deployment) - Deploy to Azure
| Component | CI/CD | Purpose |
|-----------|-------|---------|
| `.github/workflows/infrastructure.yml` | **CD Trigger** | Tells GitHub Actions when to create resources |
| `.github/workflows/deploy.yml` | **CD Trigger** | Tells GitHub Actions when to deploy |
| `deployment/pipelines/infrastructure.yml` | **CD Pipeline** | Creates Azure resources |
| `deployment/pipelines/deploy-azure-functions.yml` | **CD Pipeline** | Deploys code to Azure |
| `deployment/pipelines/azure-devops-*.yml` | **CD Pipeline** | Azure DevOps versions |
| `deployment/scripts/*.sh` | **CD Scripts** | Manual deployment |

**When it runs**:
- Infrastructure: Manual trigger (one-time setup)
- Deploy: After CI succeeds (push to main)

**What it does**:
1. ✅ Create/update Azure resources
2. ✅ Deploy application code
3. ✅ Configure settings
4. ✅ Verify deployment

---

### Quick Reference

| Component | Type | Required? | Purpose |
|-----------|------|-----------|---------|
| `.github/workflows/` | CI/CD Triggers | ✅ For GitHub Actions | Tells WHEN to run CI/CD |
| `deployment/pipelines/` | CI/CD Logic | ✅ For automation | The ACTUAL CI/CD work |
| `deployment/scripts/` | Manual CD | ⚠️ Optional | Alternative to automated CD |

---

### Full CI/CD Flow (GitHub Actions Example)

```
Developer pushes code to main branch
           ↓
┌──────────────────────────────────┐
│  CI PHASE (Continuous Integration) │
└──────────────────────────────────┘
           ↓
[Trigger] .github/workflows/build.yml
           ↓
[Execute] deployment/pipelines/build-and-test.yml
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
[Trigger] .github/workflows/deploy.yml
           ↓
[Execute] deployment/pipelines/deploy-azure-functions.yml
           ↓
    1. Download artifacts from CI
    2. Login to Azure
    3. Deploy to Azure Functions
    4. Configure app settings
    5. Verify deployment
           ↓
       CD Succeeds ✅
           ↓
    Application live on Azure! 🚀
```

---

**Simplest approach**: Pick ONE method (GitHub Actions, Azure DevOps, or Manual) and remove the others.

**Most flexible approach**: Keep all three (current structure).

**Recommended**: Keep current structure for flexibility, but feel free to simplify to Option A if you're certain you'll only use GitHub Actions.
