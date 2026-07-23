# Quick Start Guide - Nutrition Tracker

## Prerequisites
- .NET 8.0 SDK
- Node.js 18+ and npm
- SQL Server or LocalDB
- Visual Studio 2022, VS Code, or Rider

## Port Reference

| Port | Service | Protocol | Profile / Notes |
|------|---------|----------|-----------------|
| **7155** | REST API (HTTPS) | HTTPS | `--launch-profile https` — **use this one** |
| **5155** | REST API (HTTP)  | HTTP  | Always bound alongside 7155; also the only port when using the `http` profile |
| **5173** | Frontend (Vite)  | HTTPS | Default dev server port; bumps to 5174, 5175 … if already in use |
| **7071** | Azure Functions  | HTTP  | Alternative backend; only needed if running the Functions adapter |
| **10002**| Azurite Tables   | HTTP  | Local Azure Table Storage emulator; only needed for the Functions adapter |

> **Why does `dotnet run` only bind 5155?**  
> Without `--launch-profile`, the CLI picks the **first profile** listed in `launchSettings.json`, which is `http` — that profile only declares `http://localhost:5155`. To get HTTPS on 7155 you must pass `--launch-profile https` explicitly. The frontend proxy (`VITE_API_TARGET`) points to `https://localhost:7155`, so always start the API with the https profile.

### Free occupied ports before starting

Use the helper script to scan all project ports and optionally kill anything using them:

```powershell
# From repo root — dry run (shows what would be killed, kills nothing)
.\scripts\Stop-DevPorts.ps1 -WhatIf

# Interactive — lists processes then asks Y/n before killing
.\scripts\Stop-DevPorts.ps1

# Target specific ports only
.\scripts\Stop-DevPorts.ps1 -Ports 7155,5155
```

---

## One-Command Startup (Recommended)

Instead of running the backend and frontend separately, use the helper script to open both in one go:

```powershell
# From repo root — opens Backend and Frontend each in their own PowerShell window
.\scripts\Start-Dev.ps1
```

This script opens two new PowerShell windows:
- **Backend** — `dotnet run --launch-profile https` → `https://localhost:7155`
- **Frontend** — `npm run dev` → `https://localhost:5173`

> ⚠️ Make sure the database is already set up and seeded (see **Step 1** below) before running this script.

---

## Step-by-Step Setup

### 1. Setup Database

> **Schema note**: The `src` project (current hexagonal architecture) uses the **`dbo`** schema.
> The `legacy` project used the **`Nutrition`** schema. Do not mix the two.

> ⚠️ **Stop the API before running migrations.**  
> `dotnet ef` rebuilds the startup project as part of every command. If `NutritionTracker.RestApi` is already running it will hold a lock on the output DLLs and the build will fail with _"The process cannot access the file … because it is being used by another process"_. Stop the API (`Ctrl+C` in its terminal) first, then run the commands below.

```powershell
# Navigate to the SqlServer project
cd src/Adapters/Output/NutritionTracker.SqlServer

# Create initial migration (only needed once, or after model changes)
dotnet ef migrations add InitialCreate --startup-project ../../Input/NutritionTracker.RestApi

# Apply migration to database
dotnet ef database update --startup-project ../../Input/NutritionTracker.RestApi
```

#### 1a. Seed data (run in order)

All seed scripts live in `src/Adapters/Output/NutritionTracker.SqlServer/SeedData/`. They are all **idempotent** (`MERGE`-based) — safe to re-run at any time. Run them in this order to satisfy foreign-key dependencies:

```powershell
# From the repo root — adjust -S to your SQL Server instance name
$server = "HP-ZS"
$db     = "NutritionTracker"
$seed   = "src/Adapters/Output/NutritionTracker.SqlServer/SeedData"

sqlcmd -S $server -d $db -E -i "$seed/seed_food_nutrition.sql"   # 15 foods
sqlcmd -S $server -d $db -E -i "$seed/seed_users.sql"            # 3 sample users
sqlcmd -S $server -d $db -E -i "$seed/seed_food_logs.sql"        # 1 log per user
sqlcmd -S $server -d $db -E -i "$seed/seed_food_items.sql"       # 2 items per log
```

| Script | Table seeded | Idempotent key |
|---|---|---|
| `seed_food_nutrition.sql` | `FoodNutritions` | `Name` |
| `seed_users.sql` | `Users` | `Email` |
| `seed_food_logs.sql` | `FoodLogs` | `UserId + DateTime` |
| `seed_food_items.sql` | `FoodItems` | `FoodLogId + FoodNutritionId + Unit` |

### 2. Start the Backend API

```powershell
# Navigate to the API project
cd src/Adapters/Input/NutritionTracker.RestApi

# Run the API on the HTTPS profile (required — frontend proxy targets port 7155)
dotnet run --launch-profile https
```

The API will start on `https://localhost:7155` and `http://localhost:5155`.

> ⚠️ `dotnet run` without `--launch-profile https` only binds `http://localhost:5155`. The frontend proxy expects HTTPS on 7155 and will fail to reach the backend.

**Verify**: Open `https://localhost:7155/swagger` to see the API documentation.

### 3. Start the Frontend

Open a **new terminal** and run:

```powershell
# Navigate to the frontend project
cd src/Presentation/NutritionTracker.Web

# Install dependencies (first time only)
npm install

# Start development server
npm run dev
```

The frontend will start on `https://localhost:5173`.

**Verify**: Open `https://localhost:5173` in your browser.

#### Frontend port & backend target configuration

The relevant files are `src/Presentation/NutritionTracker.Web/.env` and `.env.development`:

```env
VITE_API_URL=                           # keep empty — see explanation below
VITE_API_TARGET=https://localhost:7155  # change this to switch backend
```

| Variable | Purpose |
|---|---|
| `VITE_API_URL` | Prefix injected into every `fetch` call in `api.js`. **Leave empty** so all paths stay relative (e.g. `/api/user/login`). |
| `VITE_API_TARGET` | The real backend URL the Vite proxy forwards those relative requests to. |

**Why empty `VITE_API_URL` avoids CORS:**

```
Browser  →  Vite dev server (:5173)  →  Backend (:7155 or :7071)
         ←────── response ──────────────────────────────────────
```

When `VITE_API_URL` is empty, `api.js` builds requests like `/api/user/login` — a relative path. The browser sends that to the Vite server on the **same origin** (`https://localhost:5173`), which it already trusts. Vite then silently forwards it to `VITE_API_TARGET`. From the browser's point of view it never crossed origins, so no CORS preflight is triggered. If `VITE_API_URL` were set to the backend address directly, every call would be cross-origin and the backend would need explicit CORS headers.

**To switch to the Azure Functions backend**, just change one line:
```env
VITE_API_TARGET=http://127.0.0.1:7071
```
No code changes needed.

### 4. Optional: Run Azure Functions Backend (Alternative)

Use this only if you want to run the Functions adapter instead of the REST API adapter.

```powershell
# Start Azurite Table service (required for local user signup/login)
azurite --tableHost 127.0.0.1 --tablePort 10002

# In another terminal, run Functions
cd src/Adapters/Input/NutritionTracker.AzureFunctions
func start --port 7071
```

Functions endpoints are served from `http://127.0.0.1:7071`.

If you want frontend calls to go to Functions, set `VITE_API_TARGET=http://127.0.0.1:7071` in frontend env files.

## Testing the Application

### Test Credentials (seeded by `seed_users.sql`)

Use any of the following to log in at `https://localhost:5173`:

| Username / Email | Password | Profile |
|---|---|---|
| `Demo User` or `demo@nutritiontracker.local` | `demo123` | 2 200 kcal goal |
| `Athlete User` or `athlete@nutritiontracker.local` | `athlete123` | 2 800 kcal goal |
| `Cutting User` or `cutting@nutritiontracker.local` | `cutting123` | 1 800 kcal goal |

> The login form accepts either the display name (e.g. `Demo User`) or the email address.

### 1. Using Swagger UI (API Testing)

1. Open `https://localhost:7155/swagger`
2. Test the FoodLog endpoints:
   - POST `/api/foodlog` - Create a food log
   - GET `/api/foodlog/user/{userId}` - Get user's food logs

### 2. Using the Web Interface

1. Open `https://localhost:5173`
2. Login with one of the test credentials above
3. Navigate through the app:
   - View nutrition data
   - Create food logs
   - Track daily nutrition

## Troubleshooting

### Migration fails — "file is locked by another process"

This happens when the API (`NutritionTracker.RestApi`) is already running while you try to run `dotnet ef`. The running process holds a lock on `NutritionTracker.SqlServer.dll` in the startup project's output folder.

**Fix:** Stop the API (`Ctrl+C` in its terminal) before running any `dotnet ef` command. After the migration completes you can start the API again.

### Database Connection Issues
- Check `appsettings.json` in the RestApi project
- Connection string key: `"NutritionTracker"` (not `"DefaultConnection"`)
- Make sure SQL Server is running and the server name matches (default in appsettings: `HP-ZS`)

### Azure Table Storage Issues (Functions)
- Start Azurite for local table storage before testing user signup/login
- Ensure `UseDevelopmentStorage=true` (or equivalent local table endpoint) is configured in Functions settings

### CORS Issues
- Backend is configured to allow `AllowAnyOrigin` for development
- Frontend should run on `https://localhost:5173`
- Backend should run on `https://localhost:7155`

### Port Conflicts
- **API on 5155 only (not 7155):** You ran `dotnet run` without `--launch-profile https`. Always use `dotnet run --launch-profile https`.
- **API port already in use:** Another `dotnet run` instance is still running. Stop it with `Ctrl+C` before starting a new one.
- **Frontend on 5174/5175 instead of 5173:** Port 5173 is already in use. Vite auto-increments — update `VITE_API_TARGET` proxy in `.env.development` if you also need to change the target, or just stop the other process using 5173.
- To change the API port: edit `Properties/launchSettings.json` in the RestApi project.
- To change the frontend port: edit `vite.config.js`.

### SSL Certificate Issues
```powershell
# Trust the development certificate
dotnet dev-certs https --trust
```

## Project Structure Overview

```
src/
├── Core/
│   ├── NutritionTracker.Domain        # Business entities
│   └── NutritionTracker.Application   # Use cases & ports
├── Adapters/
│   ├── Input/
│   │   └── NutritionTracker.RestApi   # Web API (Controller)
│   └── Output/
│       └── NutritionTracker.SqlServer # Database (Repository)
└── Presentation/
    └── NutritionTracker.Web           # Vue.js frontend
```

## Common Commands

### Backend
```powershell
# Build solution
dotnet build

# Run tests (when added)
dotnet test

# Create migration
dotnet ef migrations add <MigrationName> --startup-project ../../Input/NutritionTracker.RestApi

# Update database
dotnet ef database update --startup-project ../../Input/NutritionTracker.RestApi
```

### Frontend
```powershell
# Install dependencies
npm install

# Run dev server
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview

# Lint code
npm run lint
```

## Next Steps

1. **Implement remaining controllers**: User, FoodNutrition
2. **Add authentication**: JWT tokens, login/register flows
3. **Add validation**: FluentValidation in Application layer
4. **Write tests**: Unit tests for Domain and Application layers
5. **Enhance UI**: Improve Vue components and user experience

## Architecture Benefits

✅ **Clean separation** between business logic and infrastructure  
✅ **Easy to test** with mocked dependencies  
✅ **Framework independent** core domain  
✅ **Flexible** - swap database or add new adapters easily  
✅ **Maintainable** - changes isolated to specific layers
