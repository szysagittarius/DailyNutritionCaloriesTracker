# Nutrition Tracker Web Application - Troubleshooting & Fixes

## Issue Summary
The nutrition tracker web application was displaying "not supported" errors preventing proper functionality. This document outlines the root causes identified and the fixes implemented to resolve these issues.

## Date Fixed
August 18, 2025

## Root Cause Analysis

### Primary Issues Identified

1. **Port Configuration Mismatch**
   - **Problem**: Frontend Vite configuration was attempting to connect to backend on port `7155`
   - **Reality**: Backend server was running on ports `53999` (HTTPS) and `54000` (HTTP)
   - **Impact**: Frontend API calls were failing due to connection refused errors

2. **Deprecated .NET 8 JSON Serialization**
   - **Problem**: Using obsolete `JsonSerializerOptions.IgnoreNullValues` property
   - **Impact**: Build warnings and potential runtime issues with JSON serialization

3. **Missing CORS Configuration**
   - **Problem**: Cross-Origin Resource Sharing not configured for development
   - **Impact**: Browser blocking API requests from frontend (port 5173) to backend (port 53999)

4. **File Lock Conflicts**
   - **Problem**: Previous server instances holding locks on DLL files
   - **Impact**: Unable to rebuild or restart services cleanly

5. **Missing Static Files Directory**
   - **Problem**: `wwwroot` directory not found
   - **Impact**: Static file middleware warnings

## Fixes Implemented

### 1. Frontend Configuration Fix

**File**: `dailynutritioncaloriestracker.client/vite.config.js`

**Change Made**:
```javascript
// BEFORE (incorrect port)
const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
    env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'https://localhost:7155';

// AFTER (correct port)
const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
    env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'https://localhost:53999';
```

**Reasoning**: Updated the default backend URL to match the actual port configuration in `launchSettings.json` (53999/54000).

### 2. Backend JSON Serialization Fix

**File**: `DailyNutritionCaloriesTracker.Server/Program.cs`

**Change Made**:
```csharp
// BEFORE (deprecated)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.IgnoreNullValues = true;
    });

// AFTER (modern .NET 8 approach)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });
```

**Reasoning**: The `IgnoreNullValues` property was deprecated in .NET 8. Replaced with the modern `DefaultIgnoreCondition` approach to handle null value serialization.

### 3. CORS Configuration Addition

**File**: `DailyNutritionCaloriesTracker.Server/Program.cs`

**Change Made**:
```csharp
// ADDED: CORS service configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowViteDevServer",
        policy =>
        {
            policy.WithOrigins("https://localhost:5173")
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
});

// ADDED: CORS middleware activation
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowViteDevServer"); // <-- Added this line
}
```

**Reasoning**: 
- Modern web browsers enforce CORS policies for security
- The Vite dev server (port 5173) needs explicit permission to communicate with the backend API (port 53999)
- This configuration allows all HTTP methods and headers from the frontend during development

## Technical Details

### Port Configuration Summary
| Service | Protocol | Port | Purpose |
|---------|----------|------|---------|
| Backend API | HTTPS | 53999 | Main API endpoints |
| Backend API | HTTP | 54000 | Development fallback |
| Frontend Dev Server | HTTPS | 5173 | Vite development server |

### API Endpoints Affected
The following API calls were failing before the fix:
- `GET /foodnutrition/getlist` - Used by NutritionTracker component
- `GET /foodlog/GetFoodLogs` - Used by FoodLog component

### Browser Security Considerations
- CORS policy specifically configured for development environment only
- Production deployments should use more restrictive CORS policies
- Credentials are allowed to support authentication scenarios

## Verification Steps

After implementing fixes, verify the application works by:

1. **Check Backend Status**:
   ```bash
   # Navigate to Swagger documentation
   https://localhost:53999/swagger
   ```

2. **Check Frontend Status**:
   ```bash
   # Navigate to application
   https://localhost:5173
   ```

3. **Verify API Communication**:
   - Open browser developer tools (F12)
   - Navigate to Network tab
   - Reload the application
   - Confirm API calls to `/foodnutrition/getlist` and `/foodlog/GetFoodLogs` return successfully

## Build Process

### Clean Build Commands
```bash
# Stop any running processes
taskkill /f /im "NutritionTracker.Api.exe"

# Clean and rebuild backend
cd "C:\Zs_folder\NutritionTracker"
dotnet clean
dotnet build

# Start backend
dotnet run --project DailyNutritionCaloriesTracker.Server

# In separate terminal, start frontend
cd "dailynutritioncaloriestracker.client"
npm run dev
```

## Future Considerations

### Production Deployment
When deploying to production:
1. Update CORS policy to allow only production frontend domain
2. Remove development-specific CORS configurations
3. Ensure SSL certificates are properly configured
4. Update connection strings for production database

### Security Enhancements
- Implement proper API authentication/authorization
- Add request rate limiting
- Configure Content Security Policy (CSP) headers
- Use environment-specific configuration files

## Dependencies Updated
No package updates were required. All fixes involved configuration changes only.

## Testing Performed
- ✅ Backend API starts without errors
- ✅ Frontend development server connects successfully
- ✅ API endpoints respond correctly
- ✅ CORS preflight requests handled properly
- ✅ JSON serialization works as expected
- ✅ No browser console errors related to network requests

## Known Warnings (Non-blocking)
The following warnings still appear but don't affect functionality:
- Nullable reference warnings in repository layer
- Deprecated Node.js API warnings from Vite
- Missing `wwwroot` directory warnings (resolved by creating directory if needed)

## Contact
For additional troubleshooting or questions about these fixes, refer to the commit history or create an issue in the repository.

---
*This document was generated as part of troubleshooting the "not supported" errors in the Daily Nutrition Calories Tracker web application.*
