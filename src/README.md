# Nutrition Tracker - Hexagonal Architecture

## Project Structure

```
src/
├── NutritionTracker.sln
├── Core/
│   ├── NutritionTracker.Domain/           # Domain entities (pure business logic)
│   │   └── Entities/
│   │       ├── User.cs
│   │       ├── FoodLog.cs
│   │       ├── FoodItem.cs
│   │       └── FoodNutrition.cs
│   │
│   └── NutritionTracker.Application/      # Use cases and ports (interfaces)
│       ├── Ports/
│       │   └── Output/                    # Interfaces for output adapters
│       │       ├── IUserRepository.cs
│       │       ├── IFoodLogRepository.cs
│       │       └── IFoodNutritionRepository.cs
│       ├── UseCases/
│       │   └── FoodLogs/
│       │       ├── CreateFoodLogUseCase.cs
│       │       ├── GetFoodLogsByUserUseCase.cs
│       │       ├── FoodLogDto.cs
│       │       ├── Commands/
│       │       └── Queries/
│       └── DependencyInjection.cs
│
├── Adapters/
│   ├── Input/
│   │   ├── NutritionTracker.Api.Contracts/  # SHARED: Common for all input adapters
│   │   │   ├── Common/              # API responses, pagination
│   │   │   ├── Requests/            # HTTP request models → Commands
│   │   │   ├── Responses/           # DTOs → HTTP response models
│   │   │   ├── Exceptions/          # API-specific exceptions
│   │   │   └── Extensions/          # Helper methods
│   │   ├── NutritionTracker.RestApi/      # REST API adapter (ASP.NET Core Web API)
│   │   │   ├── Controllers/
│   │   │   │   └── FoodLogController.cs
│   │   │   ├── Program.cs                 # Composition Root (DI configuration)
│   │   │   └── appsettings.json
│   │   └── NutritionTracker.AzureFunctions/ # Azure Functions adapter (future)
│   │
│   └── Output/
│       └── NutritionTracker.SqlServer/    # SQL Server persistence adapter
│           ├── Data/
│           │   └── NutritionTrackerDbContext.cs
│           ├── Entities/                  # EF Core entities with annotations
│           │   ├── UserEntity.cs
│           │   ├── FoodLogEntity.cs
│           │   ├── FoodItemEntity.cs
│           │   └── FoodNutritionEntity.cs
│           ├── Mappers/
│           │   └── EntityMapper.cs        # Maps between Domain and EF entities
│           ├── Repositories/              # Implements Application ports
│           │   ├── UserRepository.cs
│           │   ├── FoodLogRepository.cs
│           │   └── FoodNutritionRepository.cs
│           └── DependencyInjection.cs
│
└── Presentation/
    └── NutritionTracker.Web/              # Vue.js frontend (to be implemented)
```

## Architecture Overview

### Hexagonal Architecture (Ports and Adapters)

This project follows the **Hexagonal Architecture** pattern with clear separation of concerns:

#### 1. **Core (Domain + Application)**
- **Domain Layer**: Pure business logic with no external dependencies
  - Entities with behavior (not anemic models)
  - Business rules and validations
  - Domain services (if needed)

- **Application Layer**: Use cases and application logic
  - **Ports**: Interfaces defining contracts for external dependencies
    - **Output Ports**: Repository interfaces for persistence
    - **Input Ports**: Use case interfaces (if needed)
  - **Use Cases**: Application-specific business logic
  - **DTOs**: Data transfer objects for communication across layers

#### 2. **Adapters**
- **Input Adapters**: Handle incoming requests
  - **Shared Contracts** (`Api.Contracts`): Common code for all input adapters
    - Request/Response models
    - Error handling
    - API wrappers
    - Validation
  - REST API (ASP.NET Core Web API)
  - Future: Azure Functions, gRPC, GraphQL, CLI, etc.

- **Output Adapters**: Handle external dependencies
  - SQL Server persistence (Entity Framework Core)
  - Future: Azure Table Storage, External APIs, etc.

#### 3. **Presentation**
- Frontend application (Vue.js)
- Communicates with Input Adapters via HTTP/REST

## Dependency Rules

```
Presentation → Input Adapters → Application ← Output Adapters
                                      ↓
                                   Domain
```

- **Domain** has NO dependencies
- **Application** depends only on Domain
- **Adapters** depend on Application (and implement its ports)
- **Presentation** depends only on Input Adapters (via HTTP)

## Key Concepts

### Ports vs Adapters
- **Ports** = Interfaces (defined in Application layer)
- **Adapters** = Implementations (in separate adapter projects)

### Composition Root
- **Program.cs** in RestApi project wires everything together
- Direct dependency injection registration (no separate Composition Root project)

### Entity Mapping
- **Domain Entities**: Rich business objects with behavior
- **EF Entities**: Persistence models with EF Core annotations
- **EntityMapper**: Converts between Domain and EF entities using reflection

## How to Run

### Prerequisites
- .NET 8.0 SDK
- SQL Server or LocalDB
- Visual Studio 2022 or VS Code

### Steps

1. **Restore packages**:
   ```powershell
   cd src
   dotnet restore
   ```

2. **Create database migration**:
   ```powershell
   cd Adapters/Output/NutritionTracker.SqlServer
   dotnet ef migrations add InitialCreate --startup-project ../../Input/NutritionTracker.RestApi
   ```

3. **Update database**:
   ```powershell
   dotnet ef database update --startup-project ../../Input/NutritionTracker.RestApi
   ```

4. **Run the API**:
   ```powershell
   cd ../../../Adapters/Input/NutritionTracker.RestApi
   dotnet run
   ```

5. **Access Swagger UI**:
   - Open browser: `https://localhost:5001/swagger` (or check console output for port)

## Running the Frontend

1. **Navigate to frontend directory**:
   ```powershell
   cd Presentation/NutritionTracker.Web
   ```

2. **Install dependencies** (first time only):
   ```powershell
   npm install
   ```

3. **Start development server**:
   ```powershell
   npm run dev
   ```

4. **Access the application**:
   - Open browser: `https://localhost:5173`

**Note**: Make sure the backend API is running on `https://localhost:7155` before starting the frontend.

## API Endpoints

### FoodLog Controller
- `GET /api/foodlog/user/{userId}` - Get all food logs for a user
- `POST /api/foodlog` - Create a new food log

### Example Request (POST /api/foodlog)
```json
{
  "dateTime": "2025-11-18T10:00:00",
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "foodItems": [
    {
      "foodNutritionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "unit": 2
    }
  ]
}
```

## Next Steps

1. **Add more use cases**:
   - Update FoodLog
   - Delete FoodLog
   - User management (login, register, update profile)
   - FoodNutrition CRUD operations

2. **Implement Vue.js frontend**:
   - Set up Vite + Vue 3
   - Connect to REST API
   - Implement UI components

3. **Add authentication**:
   - JWT token authentication
   - User authorization

4. **Add unit tests**:
   - Domain entity tests
   - Use case tests
   - Repository tests

5. **Add validation**:
   - FluentValidation for commands/queries
   - Domain entity invariants

## Benefits of This Architecture

✅ **Testability**: Core logic isolated and easy to test  
✅ **Maintainability**: Clear separation of concerns  
✅ **Flexibility**: Easy to swap adapters (e.g., change database)  
✅ **Independence**: Business logic not tied to frameworks  
✅ **Scalability**: Each layer can evolve independently
