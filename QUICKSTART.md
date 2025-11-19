# Quick Start Guide - Nutrition Tracker

## Prerequisites
- .NET 8.0 SDK
- Node.js 18+ and npm
- SQL Server or LocalDB
- Visual Studio 2022, VS Code, or Rider

## Step-by-Step Setup

### 1. Setup Database

```powershell
# Navigate to the SqlServer project
cd src/Adapters/Output/NutritionTracker.SqlServer

# Create initial migration
dotnet ef migrations add InitialCreate --startup-project ../../Input/NutritionTracker.RestApi

# Apply migration to database
dotnet ef database update --startup-project ../../Input/NutritionTracker.RestApi
```

### 2. Start the Backend API

```powershell
# Navigate to the API project
cd src/Adapters/Input/NutritionTracker.RestApi

# Run the API
dotnet run
```

The API will start on `https://localhost:7155` (or check console output for actual port).

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

## Testing the Application

### 1. Using Swagger UI (API Testing)

1. Open `https://localhost:7155/swagger`
2. Test the FoodLog endpoints:
   - POST `/api/foodlog` - Create a food log
   - GET `/api/foodlog/user/{userId}` - Get user's food logs

### 2. Using the Web Interface

1. Open `https://localhost:5173`
2. Register a new user or login
3. Navigate through the app:
   - View nutrition data
   - Create food logs
   - Track daily nutrition

## Troubleshooting

### Database Connection Issues
- Check `appsettings.json` in the RestApi project
- Default connection string: `Server=(localdb)\\mssqllocaldb;Database=NutritionTracker;Trusted_Connection=true`
- Make sure SQL Server/LocalDB is running

### CORS Issues
- Backend is configured to allow `AllowAnyOrigin` for development
- Frontend should run on `https://localhost:5173`
- Backend should run on `https://localhost:7155`

### Port Conflicts
- Backend: Check `Properties/launchSettings.json` in RestApi project
- Frontend: Modify `vite.config.js` to change port from 5173

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
