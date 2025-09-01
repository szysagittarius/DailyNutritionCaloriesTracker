# Environment Setup and CORS Troubleshooting Guide

## Overview
This guide documents the complete setup process for the Daily Nutrition Calories Tracker project on a new PC, including common issues and their solutions.

## Prerequisites Installation

### 1. Node.js Installation
The project requires Node.js for the Vue.js frontend.

**Installation Options:**
```powershell
# Option 1: Using Windows Package Manager (winget)
winget install OpenJS.NodeJS

# Option 2: Using Chocolatey
choco install nodejs

# Option 3: Using Scoop
scoop install nodejs

# Option 4: Manual download from https://nodejs.org/
```

**Verification:**
```powershell
node --version
npm --version
```

### 2. .NET Entity Framework Tools
```powershell
# Install EF Core tools globally
dotnet tool install --global dotnet-ef

# Verify installation
dotnet tool list --global
```

### 3. PowerShell Execution Policy
If you encounter npm script execution errors:
```powershell
# Fix PowerShell execution policy
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

## Database Setup

### 1. Navigate to Server Directory
```powershell
cd "DailyNutritionCaloriesTracker.Server"
```

### 2. Database Migration (for existing migrations)
```powershell
# Apply existing migrations to create database
dotnet ef database update --project ..\DNCT.Database\NT.Ef.Database.csproj
```

### 3. Create New Migration (if needed)
```powershell
# Only if starting fresh or adding new migrations
dotnet ef migrations add InitialCreate --project ..\DNCT.Database\NT.Ef.Database.csproj
```

## HTTPS Certificate Setup

### 1. Development Certificate Creation
```powershell
# Clean existing certificates
dotnet dev-certs https --clean

# Create and trust new certificate
dotnet dev-certs https --trust

# Create certificate directory
mkdir -Force "$env:APPDATA\ASP.NET\https"

# Export certificate in PEM format for Vite
dotnet dev-certs https --export-path "$env:APPDATA\ASP.NET\https\dailynutritioncaloriestracker.client.pem" --format Pem --no-password
```

## CORS Configuration Issues and Solutions

### Problem: Cross-Origin Request Blocked
**Symptoms:**
- `CORS header 'Access-Control-Allow-Origin' missing`
- `Status code: 204` or `Status code: (null)`
- `NetworkError when attempting to fetch resource`

### Root Causes:
1. API server not running
2. Incorrect CORS middleware configuration
3. Wrong middleware order in Program.cs
4. Conflicting static file middleware

### Solution: Program.cs Configuration

**Working CORS Configuration:**
```csharp
using DailyNutritionCaloriesTracker.Server.Configuration.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Add CORS policy for development
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowViteDevServer",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DefaultIgnoreCondition = 
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.RegisterDependencyInjection(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

// Configure middleware pipeline - ORDER MATTERS!
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// CORS MUST be early in the pipeline
app.UseCors("AllowViteDevServer");

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

### Critical Points:
1. **CORS middleware position**: Must be called before `UseHttpsRedirection()` and `UseAuthorization()`
2. **Remove conflicting middleware**: Comment out `UseDefaultFiles()`, `UseStaticFiles()`, and `MapFallbackToFile()` during API-only development
3. **Use permissive policy for development**: `AllowAnyOrigin()` works reliably for local development

### Alternative Restricted CORS (if needed):
```csharp
// More secure but sometimes problematic in development
policy.WithOrigins("https://localhost:5173")
      .AllowAnyMethod()
      .AllowAnyHeader()
      .AllowCredentials();
```

## Vite Configuration Issues

### Certificate Creation Errors
If Vite fails to create certificates, update `vite.config.js`:

```javascript
// Enhanced certificate creation with better error handling
if (!fs.existsSync(baseFolder)) {
    fs.mkdirSync(baseFolder, { recursive: true });
}

if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
    console.log('Creating development certificate...');
    const result = child_process.spawnSync('dotnet', [
        'dev-certs',
        'https',
        '--export-path',
        certFilePath,
        '--format',
        'Pem',
        '--no-password',
    ], { stdio: 'pipe' });
    
    if (result.status !== 0) {
        console.error('Failed to create certificate:', result.stderr?.toString());
        console.log('You may need to run: dotnet dev-certs https --trust');
        process.exit(1);
    }
    console.log('Certificate created successfully!');
}
```

## Running the Application

### 1. Start API Server
```powershell
cd "DailyNutritionCaloriesTracker.Server"
dotnet run
```

**Expected output:**
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7155
```

### 2. Start Frontend Development Server
```powershell
cd "dailynutritioncaloriestracker.client"
npm run dev
```

**Expected output:**
```
VITE ready in XXXms
Local:   https://localhost:5173/
```

## Verification Steps

### 1. Check Server Status
```powershell
# Verify API server is running
netstat -an | findstr "7155"
```

### 2. Test API Endpoints
- Browse to: `https://localhost:7155/swagger`
- Test endpoints through Swagger UI

### 3. Test Frontend
- Browse to: `https://localhost:5173`
- Verify login functionality works

## Common Troubleshooting Commands

### Server Issues
```powershell
# Build without client dependencies
dotnet build --verbosity minimal /p:SpaRoot="" /p:SpaProxyServerUrl=""

# Check for compile errors
dotnet build

# Clean and rebuild
dotnet clean
dotnet build
```

### Certificate Issues
```powershell
# Recreate certificates
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

### Database Issues
```powershell
# Update database
dotnet ef database update --project ..\DNCT.Database\NT.Ef.Database.csproj

# List migrations
dotnet ef migrations list --project ..\DNCT.Database\NT.Ef.Database.csproj
```

## File Locations

### Important Configuration Files
- **API Configuration**: `DailyNutritionCaloriesTracker.Server/Program.cs`
- **Database Connection**: `DailyNutritionCaloriesTracker.Server/appsettings.json`
- **Frontend Configuration**: `dailynutritioncaloriestracker.client/vite.config.js`
- **Launch Settings**: `DailyNutritionCaloriesTracker.Server/Properties/launchSettings.json`

### Certificate Locations
- **Windows**: `%APPDATA%\ASP.NET\https\`
- **Certificate Files**: 
  - `dailynutritioncaloriestracker.client.pem`
  - `dailynutritioncaloriestracker.client.key`

## Development URLs
- **API Server**: `https://localhost:7155`
- **Frontend**: `https://localhost:5173`
- **Swagger UI**: `https://localhost:7155/swagger`

## Notes for Production

When deploying to production:
1. Replace `AllowAnyOrigin()` with specific domain restrictions
2. Re-enable static file middleware if needed
3. Use environment-specific CORS policies
4. Configure proper SSL certificates

## Troubleshooting Checklist

- [ ] Node.js installed and accessible
- [ ] .NET EF tools installed globally
- [ ] PowerShell execution policy allows scripts
- [ ] Development certificates created and trusted
- [ ] Database migrations applied
- [ ] API server running on port 7155
- [ ] Frontend server running on port 5173
- [ ] CORS middleware properly configured
- [ ] No conflicting static file middleware

---

*Last updated: August 31, 2025*
*This guide documents the successful setup process for a new PC development environment.*
