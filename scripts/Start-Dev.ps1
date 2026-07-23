<#
.SYNOPSIS
    Starts the Nutrition Tracker backend API and frontend dev server in separate windows.
.DESCRIPTION
    Opens two PowerShell windows:
      Backend  — dotnet run --launch-profile https  => https://localhost:7155
      Frontend — npm run dev                         => https://localhost:5173
.EXAMPLE
    .\scripts\Start-Dev.ps1
#>

$root   = Split-Path -Parent $PSScriptRoot
$apiDir = Join-Path $root "src\Adapters\Input\NutritionTracker.RestApi"
$webDir = Join-Path $root "src\Presentation\NutritionTracker.Web"

Write-Host ""
Write-Host "=== Nutrition Tracker — Dev Startup ===" -ForegroundColor Cyan
Write-Host ""

# --- Backend ---
Write-Host "  [1] Backend API  -> https://localhost:7155" -ForegroundColor Green
Start-Process powershell -ArgumentList @(
    "-NoExit",
    "-Command",
    "cd '$apiDir'; Write-Host '--- Backend API ---' -ForegroundColor Green; dotnet run --launch-profile https"
)

# --- Frontend ---
Write-Host "  [2] Frontend     -> https://localhost:5173" -ForegroundColor Yellow
Start-Process powershell -ArgumentList @(
    "-NoExit",
    "-Command",
    "cd '$webDir'; Write-Host '--- Frontend ---' -ForegroundColor Yellow; npm run dev"
)

Write-Host ""
Write-Host "Both services are starting in separate windows." -ForegroundColor Cyan
Write-Host ""
Write-Host "  Backend API : https://localhost:7155"
Write-Host "  Swagger UI  : https://localhost:7155/swagger"
Write-Host "  Frontend    : https://localhost:5173"
Write-Host ""
Write-Host "Press Ctrl+C in each window to stop the respective service." -ForegroundColor DarkGray
