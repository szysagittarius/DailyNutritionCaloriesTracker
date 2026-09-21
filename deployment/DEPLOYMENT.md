# Azure Deployment Guide

Deploy the Nutrition Tracker (`src/` — not `legacy/`) to Azure using **GitHub Actions** (already set up
in [.github/workflows/](../.github/workflows)) — no Azure DevOps needed since the repo lives on GitHub.

- **Frontend**: Azure Static Web Apps (Free tier)
- **Backend**: Azure Functions (Consumption plan — free tier)
- **Database**: Azure Table Storage
- **Estimated cost**: ~$0.05–$2.00/month

## ✅ Setup Checklist (do this once)

### 1. Install Azure CLI & sign in
```powershell
winget install --id Microsoft.AzureCLI -e
az login
```
If sign-in hangs or fails with an MFA error, try `az login --use-device-code`, or scope it to the
right tenant with `az login --tenant <tenant-id>` (find your subscription's tenant at
[portal.azure.com](https://portal.azure.com) → Subscriptions).

### 2. Create a service principal for GitHub Actions
```powershell
$subId = az account show --query id -o tsv
az ad sp create-for-rbac --name "github-nutrition-tracker-deploy" --role Contributor --scopes /subscriptions/$subId --sdk-auth
```
Copy the full JSON it prints.

### 3. Add the `AZURE_CREDENTIALS` secret
GitHub repo → **Settings → Secrets and variables → Actions → New repository secret**
- Name: `AZURE_CREDENTIALS`
- Value: paste the **entire JSON** from Step 2, exactly as printed (starts with `{`, ends with `}` —
  no extra text or code-fence markers, or the login step will fail with a JSON parse error)

### 4. Run the "Deploy Infrastructure" workflow
GitHub repo → **Actions** → **Deploy Infrastructure** → **Run workflow** → environment `dev` → Run.

This creates (safe to re-run):
- Resource Group `rg-nutrition-tracker-dev`
- Storage Account `stnutritiontrackerdev` (Table Storage)
- Function App `func-nutrition-tracker-dev` (Consumption/free)
- Static Web App `swa-nutrition-tracker-dev` (Free tier)

When it finishes, open the run and download two artifacts: **`publish-profile`** and
**`static-web-app-token`**.

### 5. Add the remaining secrets + one variable
| Name | Type | Value |
|---|---|---|
| `AZURE_FUNCTIONAPP_PUBLISH_PROFILE` | Secret | full contents of the downloaded `publish-profile.xml` |
| `AZURE_STATIC_WEB_APPS_API_TOKEN` | Secret | full contents of the downloaded `swa-token.txt` |
| `FUNCTION_APP_URL` | **Variable** (Actions → Variables tab, not Secrets) | `https://func-nutrition-tracker-dev.azurewebsites.net` |

### 6. Deploy the app
Push any commit touching `src/` to `main` — or trigger manually from the **Actions** tab:
- **Deploy Backend (Azure Functions)** — builds, tests, and deploys the API
- **Deploy Frontend (Static Web App)** — builds the Vue app and deploys it

### 7. Verify
```powershell
curl https://func-nutrition-tracker-dev.azurewebsites.net/api/users
```
Then open the Static Web App URL (shown in the infrastructure run's summary) in a browser.

---

## How the pipeline is organized

```
.github/workflows/       # The real, self-contained GitHub Actions workflows
├── build.yml                 # CI — build + test, on every push/PR to main
├── deploy-backend.yml        # CD — deploy Azure Functions, on push to main (backend paths only)
├── deploy-frontend.yml       # CD — deploy Static Web App, on push to main (frontend paths only)
└── infrastructure.yml        # CD — create/update Azure resources (manual, or when infra files change)
deployment/
├── pipelines/azure-devops-*.yml   # Azure DevOps equivalents (only needed if you switch off GitHub)
└── scripts/                       # Manual bash scripts (fallback if you prefer CLI over Actions)
```

Full background on why it's structured this way: [ARCHITECTURE.md](ARCHITECTURE.md).
Trigger behavior and required secrets: [GITHUB_ACTIONS_SAFETY.md](GITHUB_ACTIONS_SAFETY.md).

## Frontend → Backend wiring

The frontend reads its API URL from `VITE_API_URL` at build time (see
`src/Presentation/NutritionTracker.Web/src/services/api.js`) — no code changes needed. In CI,
`deploy-frontend.yml` sets `VITE_API_URL` from the `FUNCTION_APP_URL` repository variable.

## Troubleshooting

| Symptom | Fix |
|---|---|
| `az : term not recognized` | Reopen the terminal (PATH updated after install), or run `$env:Path = [System.Environment]::GetEnvironmentVariable("Path","Machine") + ";" + [System.Environment]::GetEnvironmentVariable("Path","User")` |
| `az login` hangs / no popup | Use `az login --use-device-code`, complete it at https://microsoft.com/devicelogin |
| `AADSTS50076` (MFA required) during device-code login | Use regular `az login --tenant <tenant-id>` instead (interactive browser handles MFA properly) |
| "No subscriptions found" | You're signed into the wrong tenant — check **portal.azure.com → Subscriptions** for the correct tenant ID, then `az login --tenant <tenant-id>` |
| Azure login step in the workflow fails with a JSON parse error | The `AZURE_CREDENTIALS` secret has extra text/formatting around the JSON — re-paste it exactly as printed by `az ad sp create-for-rbac` |
| Backend deploy fails: `Failed to fetch Kudu App Settings. Unauthorized (CODE: 401)` | Azure disables SCM Basic Auth Publishing by default on new Function Apps, which breaks publish-profile deploys. Fix: `az resource update --resource-group rg-nutrition-tracker-dev --name scm --namespace Microsoft.Web --resource-type basicPublishingCredentialsPolicies --parent sites/func-nutrition-tracker-dev --set properties.allow=true` (already automated in `infrastructure.yml`/`deploy-infrastructure.sh` going forward). If it still 401s after that, the `AZURE_FUNCTIONAPP_PUBLISH_PROFILE` secret is stale (e.g. downloaded before the policy was enabled) — refetch with `az functionapp deployment list-publishing-profiles --name func-nutrition-tracker-dev --resource-group rg-nutrition-tracker-dev --xml` and re-paste it |
| "Build and Test" job fails even though build/tests pass, annotation says `Could not find any files for **/test-results.trx` | `src/NutritionTracker.sln` currently has no test projects, so `dotnet test` produces no `.trx` and the `EnricoMi/publish-unit-test-result-action` reporting step fails the job over a missing file. Fixed by adding `continue-on-error: true` to that step in `build.yml` — it's just a reporting step, not the actual build/test result |
| Frontend deploy fails quickly with no clear error | Usually a stale/incorrectly-pasted `AZURE_STATIC_WEB_APPS_API_TOKEN`. Regenerate with `az staticwebapp secrets list --name swa-nutrition-tracker-dev --resource-group rg-nutrition-tracker-dev --query "properties.apiKey" -o tsv` and re-paste it as the secret |
| Frontend deploy step reports **success**, but the live site is blank with a browser console error like `Uncaught TypeError: Failed to resolve module specifier "vue"` | The deployed site is serving the raw source `index.html` (`<script src="/src/main.js">`) instead of the built `dist/` bundle — confirm by requesting the hashed asset shown in the CI build log (e.g. `/assets/index-XXXX.js`); a `404` proves the wrong content was deployed even though the workflow said "success". Root cause in this repo: `vite.config.js` ran `dotnet dev-certs https` unconditionally (even during `vite build`), which fails in Azure's Oryx build container (no `dotnet` CLI) and either crashes the build outright or causes the deploy action to silently fall back to zipping the raw source folder instead of `dist`. Fix: only generate the dev cert / HTTPS server config when Vite's `command === 'serve'` (see `defineConfig(({ command }) => {...})` in `vite.config.js`), never for `build`. Also avoid `skip_app_build: true` + a separate manual `npm run build` step for this app — it deployed the wrong content silently in several different `app_location`/`output_location` combinations; let the `Azure/static-web-apps-deploy@v1` action's own Oryx build run `npm ci && npm run build` instead (pass `VITE_API_URL` via the step's `env:`, it's forwarded into the build container automatically) |
| CORS errors calling the API from the browser | `az functionapp cors add --name func-nutrition-tracker-dev --resource-group rg-nutrition-tracker-dev --allowed-origins "https://<your-static-web-app-hostname>"` |
| Function App not responding | `az functionapp restart --name func-nutrition-tracker-dev --resource-group rg-nutrition-tracker-dev`, then check `az webapp log tail --name func-nutrition-tracker-dev --resource-group rg-nutrition-tracker-dev` |
| Need to see full GitHub Actions run/job logs (not just public annotations) | Install GitHub CLI (`winget install --id GitHub.cli -e`), then `gh auth login --web`, then `gh run view <run-id> --log` or `gh run view <run-id> --log-failed` to pull the exact failing step's output |

## Verifying on the Azure Portal

1. **portal.azure.com** → search **"rg-nutrition-tracker-dev"** (Resource Group) → confirm you see: `func-nutrition-tracker-dev`, `stnutritiontrackerdev`, `swa-nutrition-tracker-dev`, plus a `stfunc...` storage account.
2. Open **`func-nutrition-tracker-dev`** → left menu **Functions** → after a successful deploy you should see function names (e.g. Users, FoodLog, FoodNutrition) listed. Click **Log stream** to watch live logs while testing.
3. Test an endpoint directly: `https://func-nutrition-tracker-dev.azurewebsites.net/api/users` (browser or `curl`).
4. Open **`stnutritiontrackerdev`** → **Storage browser** → **Tables** — after the first successful API call, you should see `Users`/`FoodLogs`/`FoodNutrition` tables auto-created.
5. Open **`swa-nutrition-tracker-dev`** → grab the **URL** on the Overview page → open it in a browser to see the live frontend; use browser DevTools → Network tab to confirm it's calling the Function App URL successfully (watch for CORS errors here specifically).
6. If CORS errors appear, tighten it: `az functionapp cors add --name func-nutrition-tracker-dev --resource-group rg-nutrition-tracker-dev --allowed-origins "https://<your-static-web-app-hostname>"`.
7. If the page is blank or DevTools shows a `Failed to resolve module specifier` error, view page source (or `curl` the URL) — the `<script>` tag should point at a hashed file like `/assets/index-XXXX.js`, **not** `/src/main.js`. If it's the latter, the wrong (raw source) content was deployed; see the troubleshooting table above.

## Alternative: Azure DevOps instead of GitHub Actions

Only relevant if you move off GitHub. Point Azure DevOps pipelines at:
`deployment/pipelines/azure-devops-build.yml`, `azure-devops-infrastructure.yml`, `azure-devops-deploy.yml`,
with an Azure Resource Manager service connection (Contributor role) set as variable `azureServiceConnection`.
See [README.md](README.md) for details.

## Alternative: Manual deployment via scripts (no GitHub Actions)

```powershell
cd deployment/scripts
./create-service-principal.sh          # one-time
./deploy-infrastructure.sh dev eastus  # creates Azure resources
./deploy-application.sh dev            # deploys the Functions app
```
(These `.sh` scripts require a bash shell — WSL, Git Bash, or macOS/Linux.)

## Local Development with Azurite

```powershell
npm install -g azurite
azurite --silent --location c:\azurite --debug c:\azurite\debug.log

# in another terminal
cd src/Adapters/Input/NutritionTracker.AzureFunctions
func start
```
Uses `UseDevelopmentStorage=true` from `local.settings.json`.

## Cost Estimation

| Service | Free allowance | Beyond free tier |
|---|---|---|
| Azure Functions (Consumption) | 1M requests + 400K GB-s/month, forever | $0.20/million executions |
| Azure Table Storage | 5GB + 20K ops free for 12 months | ~$0.045/GB/month + $0.00036/10K ops |
| Azure Static Web Apps (Free SKU) | 100GB bandwidth/month, forever | N/A — no paid tier needed |

**Estimated total**: $0–0.05/month in year one, ~$0.50–2.00/month after.

## FAQ: Is everything really free, forever?

**Q: Are the GitHub Actions workflows free?**
A: Yes, with no time limit — this repo is a **public** GitHub repository, and GitHub does not meter
Actions minutes at all for public repos (private repos get 2,000 free minutes/month, then billed).
Verified with `gh repo view --json visibility` → `PUBLIC`.

**Q: Are the Azure resources free forever, or just for a trial period?**
A: It depends on the resource — this subscription (`Azure subscription 1`, quota ID
`PayAsYouGo_2014-09-01`) is a standard **Pay-As-You-Go** subscription, not a brand-new Free
Trial/Student account, so it does **not** get the "free for 12 months" new-account credits.
What actually applies here:
- **Azure Functions (Consumption plan)** — genuinely free forever, no time limit. The 1M
  requests + 400,000 GB-s/month grant is part of Azure's **Always Free** tier and applies to
  every subscription (including Pay-As-You-Go), every month, indefinitely. You're only billed if
  you exceed that grant in a given month.
- **Azure Static Web Apps (Free SKU)** — also **Always Free**, no time limit, as long as you stay
  within the tier's limits (100GB bandwidth/month, etc.). This is a permanent $0 pricing tier, not
  a trial.
- **Azure Table Storage** — **not** free. Storage accounts are billed pay-as-you-go for capacity
  and transactions from day one on this subscription type (the "5GB + 20K ops free for 12 months"
  row above only applies to new Free Trial/Azure-for-Students subscriptions). In practice, a
  single-user hobby project's storage/transaction volume costs a few **cents per month**, not $0.
- **Application Insights / Log Analytics** (created alongside the Function App) has its own
  Always Free ingestion allowance (5GB/month) — fine for this project's traffic, but would start
  billing if log volume grew significantly.

**Bottom line**: no component here has a hard expiration date or a "trial ends, then you get
charged full price" cliff. The Consumption Functions plan and the Static Web App are genuinely
free forever at this usage level; Table Storage is billed but at fractions of a cent/month for a
personal project. Keep an eye on the Azure Portal's **Cost Management + Billing** blade if you're
ever unsure.

## Security Best Practices

1. Use Managed Identity where possible instead of connection strings
2. Enable Application Insights for monitoring
3. Restrict CORS to the Static Web App's actual domain (not `*`) once it's live
4. Use Azure Key Vault for secrets in production
5. Enable HTTPS only
6. Implement authentication (Azure AD B2C recommended)

## Next Steps After Going Live

1. ⏭️ Setup custom domain (optional, free on Static Web Apps)
2. ⏭️ Configure Application Insights
3. ⏭️ Implement authentication
4. ⏭️ Create staging/production environments (run Infrastructure workflow with that environment selected)
