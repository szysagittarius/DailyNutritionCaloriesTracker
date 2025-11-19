# GitHub Actions Safety Guide

## ⚠️ Important: Auto-Trigger Behavior

When you commit and push code to `.github/workflows/`, GitHub Actions will **automatically** detect and activate the workflows. There's no manual setup required.

## What Happens When You Push?

### Scenario 1: Push `.github/workflows/build.yml`

```yaml
# .github/workflows/build.yml
on:
  push:
    branches: [main, develop]
```

**Result**: 
- ✅ Workflow activates immediately after push
- ⚠️ **Will run on EVERY push to main or develop**
- Builds and tests your code automatically
- **Cost**: Uses GitHub Actions minutes (2,000 free/month for public repos)

### Scenario 2: Push `.github/workflows/deploy.yml`

```yaml
# .github/workflows/deploy.yml
on:
  push:
    branches: [main]
```

**Result**:
- ✅ Workflow activates immediately after push
- ⚠️ **Will deploy to Azure on EVERY push to main**
- Could deploy broken code if CI didn't run first
- **Cost**: Uses GitHub Actions minutes + Azure resources

### Scenario 3: Push `.github/workflows/infrastructure.yml`

```yaml
# .github/workflows/infrastructure.yml
on:
  workflow_dispatch:  # Manual trigger only
```

**Result**:
- ✅ Workflow registered but **does NOT auto-run**
- ✅ Safe - only runs when you click "Run workflow" in GitHub UI
- Good for one-time setup tasks

## How to Safely Set Up Workflows

### Option A: Start with Manual Triggers (Recommended)

**Step 1**: Initially use `workflow_dispatch` for all workflows:

```yaml
# .github/workflows/build.yml
name: Build and Test
on:
  workflow_dispatch:  # ✅ Safe - manual only
```

```yaml
# .github/workflows/deploy.yml
name: Deploy
on:
  workflow_dispatch:  # ✅ Safe - manual only
```

**Step 2**: Test manually in GitHub UI:
1. Go to repository → Actions tab
2. Select workflow
3. Click "Run workflow"
4. Verify it works correctly

**Step 3**: Once tested, enable automatic triggers:

```yaml
# .github/workflows/build.yml
name: Build and Test
on:
  workflow_dispatch:  # Keep for manual testing
  push:              # Add automatic trigger
    branches: [main, develop]
```

### Option B: Use Path Filters

Only run workflows when relevant files change:

```yaml
# .github/workflows/build.yml
on:
  push:
    branches: [main]
    paths:
      - 'src/**'                    # Only C# code
      - '**.csproj'                 # Only project files
      - 'deployment/pipelines/**'   # Only pipeline changes
      # Ignore documentation changes
      - '!docs/**'
      - '!**.md'
```

### Option C: Disable Until Ready

**Method 1**: Rename file
```bash
# Disable
git mv .github/workflows/build.yml .github/workflows/build.yml.disabled

# Enable later
git mv .github/workflows/build.yml.disabled .github/workflows/build.yml
```

**Method 2**: Keep in local branch
```bash
# Work on separate branch
git checkout -b setup-ci-cd

# Push workflow files to branch (won't trigger on main)
git add .github/workflows/
git commit -m "Add CI/CD workflows"
git push origin setup-ci-cd

# Test in branch, merge to main when ready
```

## Required Secrets Setup

Before workflows can deploy to Azure, you need GitHub Secrets configured:

### Check if Secrets Exist

Go to: **Repository → Settings → Secrets and variables → Actions**

### Required Secrets

| Secret Name | Required For | What Happens Without It |
|-------------|--------------|-------------------------|
| `AZURE_CREDENTIALS` | All Azure deployments | ❌ Workflow fails at Azure login |
| `AZURE_FUNCTIONAPP_PUBLISH_PROFILE` | Application deployment | ❌ Workflow fails at deploy step |
| `AZURE_RESOURCE_GROUP` | Infrastructure & Deploy | ❌ Workflow fails - can't find resources |

### Safe Testing Without Secrets

**Option 1**: Test locally first
```bash
# Run build locally (no secrets needed)
cd src
dotnet build
dotnet test

# Only push workflows after local success
```

**Option 2**: Use `if` conditions
```yaml
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - name: Build
        run: dotnet build
      
      - name: Deploy
        if: ${{ secrets.AZURE_CREDENTIALS != '' }}  # ✅ Skip if no secrets
        run: # deploy commands
```

## Current Repository Status

### What You Have Now

```
.github/workflows/
├── build.yml          ⚠️ Will trigger on: push to main/develop
├── deploy.yml         ⚠️ Will trigger on: push to main
└── infrastructure.yml ✅ Safe: workflow_dispatch only (manual)

deployment/pipelines/
├── build-and-test.yml           ← Actual CI logic (called by build.yml)
├── deploy-azure-functions.yml   ← Actual CD logic (called by deploy.yml)
└── infrastructure.yml            ← Actual infra logic (called by infrastructure.yml)
```

### What Happens When You Push This Repository

1. **First Push to Main**:
   ```
   git add .github/workflows/
   git commit -m "Add CI/CD workflows"
   git push origin main
   ```
   
   **Result**:
   - ⚠️ `build.yml` will trigger immediately (detects push to main)
   - ⚠️ May fail if you don't have secrets configured
   - ✅ `infrastructure.yml` will NOT trigger (manual only)
   - ⚠️ `deploy.yml` will trigger if build succeeds

2. **Subsequent Pushes to Main**:
   - Every push triggers build + deploy
   - Could rack up GitHub Actions minutes
   - Could deploy broken code

## Recommended First-Time Setup

### Step 1: Initial Push with Safe Configuration

**Before pushing**, modify workflows to be manual-only:

```yaml
# .github/workflows/build.yml (temporarily disable auto-trigger)
on:
  workflow_dispatch:
  # push:           ← Comment out
  #   branches:     ← Comment out
  #     - main      ← Comment out
```

### Step 2: Configure Secrets

1. Create Azure service principal:
   ```bash
   cd deployment/scripts
   ./create-service-principal.sh
   ```

2. Add secrets to GitHub (Settings → Secrets)

### Step 3: Test Manually

1. Go to Actions tab
2. Run "Deploy Infrastructure" workflow
3. Run "Build and Test" workflow
4. Run "Deploy" workflow

### Step 4: Enable Auto-Triggers

Once everything works, uncomment:

```yaml
# .github/workflows/build.yml
on:
  workflow_dispatch:
  push:              # ← Uncomment
    branches:        # ← Uncomment
      - main         # ← Uncomment
```

## Quick Reference

| Trigger Type | When It Runs | Safe for First Push? |
|--------------|--------------|---------------------|
| `workflow_dispatch` | Manual only | ✅ Yes - requires manual click |
| `push: branches: [main]` | Every push to main | ⚠️ No - runs immediately |
| `pull_request` | When PR created | ✅ Yes - only on PRs |
| `schedule` | On cron schedule | ⚠️ No - runs on schedule |

## Costs and Limits

### GitHub Actions Minutes (Free Tier)

- Public repos: **Unlimited** free minutes
- Private repos: **2,000** minutes/month free
- After free tier: $0.008 per minute

### Our Workflows Usage

| Workflow | Duration | Minutes Used | Cost (Private Repo) |
|----------|----------|--------------|---------------------|
| Build & Test | ~3 min | 3 | $0.024 |
| Deploy | ~5 min | 5 | $0.040 |
| Infrastructure | ~2 min | 2 | $0.016 |

**Per deployment cycle**: ~10 minutes (~$0.08 if private)

**Monthly estimate** (5 deployments/week): ~200 minutes (~$1.60 if private)

## Disable Workflows Completely

If you want to keep files but prevent execution:

**Method 1**: Add to all workflows
```yaml
on:
  workflow_dispatch:  # Only manual
  # Remove all other triggers
```

**Method 2**: Move to different folder
```bash
git mv .github/workflows .github/workflows.disabled
```

**Method 3**: Delete from repository
```bash
git rm -r .github/workflows/
git commit -m "Remove workflows temporarily"
```

Can restore from git history later:
```bash
git checkout <commit-hash> -- .github/workflows/
```

## Summary

✅ **Safe to push**:
- Workflows with `workflow_dispatch` only
- Documentation files
- Scripts in `deployment/scripts/`
- Pipeline files in `deployment/pipelines/`

⚠️ **Will auto-trigger**:
- Workflows with `on: push`
- Workflows with `on: pull_request`
- Could fail without secrets
- Could rack up minutes

🎯 **Recommended approach**:
1. Start with `workflow_dispatch` only
2. Configure secrets
3. Test manually
4. Enable auto-triggers when ready
