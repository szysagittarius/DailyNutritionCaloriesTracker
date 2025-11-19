# Daily Nutrition Calories Tracker - Hexagonal Architecture Implementation

## Project Overview
Migration of Daily Nutrition Calories Tracker to hexagonal (ports and adapters) architecture with clean separation of concerns and dependency injection.

---

## Table of Contents
1. [What is Hexagonal Architecture?](#what-is-hexagonal-architecture)
2. [Architecture Layers](#architecture-layers)
3. [Folder Structure](#complete-folder-structure)
4. [Dependency Injection Setup](#dependency-injection-setup)
5. [Ports vs Adapters - Clarification](#ports-vs-adapters-clarification)
6. [Implementation Guide](#implementation-guide)
7. [Migration from Existing Code](#migration-from-existing-codebase)
8. [Commands Reference](#commands-reference)
9. [Troubleshooting](#troubleshooting)

---

## What is Hexagonal Architecture?

A software design pattern that:
- **Isolates business logic** from infrastructure (databases, APIs, UI)
- **Makes code testable** - easy to swap real implementations with mocks
- **Enables flexibility** - switch databases or APIs without changing business logic
- **Follows Dependency Inversion** - all dependencies point inward to the core

### Visual Representation

```
        External World
             │
    ┌────────▼────────┐
    │  Input Adapter  │  (REST API, Azure Functions)
    │  "How external  │
    │   calls app"    │
    └────────┬────────┘
             │
    ┌────────▼────────┐
    │  Input Port     │  (ILogMealUseCase interface)
    │  "What app      │
    │   can do"       │
    └────────┬────────┘
             │
    ┌────────▼─────────────────┐
    │   Application            │
    │   (Business Logic)       │
    │                          │
    │   Uses Output Ports ─────┼──┐
    └────────┬─────────────────┘  │
             │                    │
             ▼                    │
    ┌────────────────┐  ┌────────▼────────┐
    │     Domain     │  │  Output Port    │  (IUserRepository)
    │   (Entities)   │  │  "What app      │
    │                │  │   needs"        │
    └────────────────┘  └────────┬────────┘
                                 │
                        ┌────────▼────────┐
                        │  Output Adapter │  (SQL Server, SendGrid)
                        │  "How app calls │
                        │   external"     │
                        └─────────────────┘
                                 │
                        External World (Database, Email)
```

### Dependency Flow

```
┌─────────────────────────────────────────────┐
│        Dependency Flow (Always Inward)      │
│                                             │
│  Input Adapters          Output Adapters   │
│  (REST API)              (SQL Server)      │
│       │                        │           │
│       └────────┐    ┌──────────┘           │
│                ▼    ▼                      │
│           Application  ──────▶  Domain    │
│           (Use Cases)        (Entities)    │
│                                             │
└─────────────────────────────────────────────┘
```

**Key Rule**: All dependencies point INWARD. Outer layers depend on inner layers, never the reverse.

---

## Architecture Layers

### 1. Domain Layer (Core Business Logic)
**Location**: `src/Core/NutritionTracker.Domain`  
**Dependencies**: NONE  
**Project File**: `NutritionTracker.Domain.csproj`

**Contains**:
- **Entities**: Core business objects (`User`, `FoodItem`, `NutritionEntry`, `Recipe`)
- **Value Objects**: Immutable objects (`Email`, `MacroNutrients`, `ServingSize`)
- **Domain Services**: Pure business logic (`CalorieCalculationService`)
- **Domain Events**: Business events (`MealLoggedEvent`, `GoalAchievedEvent`)
- **Domain Exceptions**: Business rule violations

**Example Code**:
```csharp
// EntityBase - Base for all entities
namespace NutritionTracker.Domain.Entities;

public class EntityBase
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}

// FoodItem - Domain entity
public class FoodItem : EntityBase
{
    public required string Name { get; set; }
    public required string Measurement { get; set; }
    public double Calories { get; set; }
    public double Protein { get; set; }
    public double Carbs { get; set; }
    public double Fat { get; set; }
    
    // Domain logic
    public MacroNutrients GetMacros() => new(Protein, Carbs, Fat);
}

// Value Object
public record MacroNutrients(double Protein, double Carbs, double Fat)
{
    public double TotalCalories => (Protein * 4) + (Carbs * 4) + (Fat * 9);
}
```

**Project Structure**:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <!-- NO PROJECT REFERENCES - Domain is pure -->
</Project>
```

---

### 2. Application Layer (Use Cases & Ports)
**Location**: `src/Core/NutritionTracker.Application`  
**Dependencies**: Domain only  
**Project File**: `NutritionTracker.Application.csproj`

**Contains**:

#### A. Ports (Interfaces)
```
Application/Ports/
├── Input/                    # What the application CAN DO (use cases)
│   ├── IRegisterUserUseCase.cs
│   ├── ILogMealUseCase.cs
│   └── IGetDailyTotalsQuery.cs
│
└── Output/                   # What the application NEEDS (dependencies)
    ├── IUserRepository.cs
    ├── IFoodRepository.cs
    └── IEmailSender.cs
```

#### B. Use Cases (Implementations)
```csharp
// Input Port (Interface)
namespace NutritionTracker.Application.Ports.Input.Nutrition;

public interface ILogMealUseCase
{
    Task<NutritionEntryDto> ExecuteAsync(LogMealCommand command);
}

// Use Case Implementation
namespace NutritionTracker.Application.UseCases.Nutrition;

public class LogMealUseCase : ILogMealUseCase
{
    private readonly IUserRepository _userRepo;
    private readonly IFoodRepository _foodRepo;
    private readonly INutritionEntryRepository _nutritionRepo;
    
    public LogMealUseCase(
        IUserRepository userRepo,
        IFoodRepository foodRepo,
        INutritionEntryRepository nutritionRepo)
    {
        _userRepo = userRepo;
        _foodRepo = foodRepo;
        _nutritionRepo = nutritionRepo;
    }
    
    public async Task<NutritionEntryDto> ExecuteAsync(LogMealCommand command)
    {
        // 1. Validate user exists
        var user = await _userRepo.GetByIdAsync(command.UserId);
        if (user == null) throw new UserNotFoundException(command.UserId);
        
        // 2. Get food item
        var food = await _foodRepo.GetByIdAsync(command.FoodItemId);
        if (food == null) throw new FoodNotFoundException(command.FoodItemId);
        
        // 3. Create nutrition entry
        var entry = new NutritionEntry
        {
            UserId = command.UserId,
            FoodItemId = command.FoodItemId,
            Quantity = command.Quantity,
            MealType = command.MealType,
            ConsumedAt = command.ConsumedAt
        };
        
        // 4. Save to repository
        await _nutritionRepo.CreateAsync(entry);
        
        // 5. Return DTO
        return new NutritionEntryDto
        {
            Id = entry.Id,
            FoodName = food.Name,
            Calories = food.Calories * command.Quantity,
            // ... other properties
        };
    }
}
```

#### C. Commands & Queries (CQRS Pattern)
```csharp
// Command - Write operation
public record LogMealCommand(
    Guid UserId,
    Guid FoodItemId,
    double Quantity,
    string MealType,
    DateTime ConsumedAt
);

// Query - Read operation
public record GetDailyTotalsQuery(
    Guid UserId,
    DateTime Date
);
```

#### D. DTOs (Data Transfer Objects)
```csharp
public class NutritionEntryDto
{
    public Guid Id { get; set; }
    public string FoodName { get; set; } = string.Empty;
    public double Calories { get; set; }
    public double Protein { get; set; }
    public double Carbs { get; set; }
    public double Fat { get; set; }
    public string MealType { get; set; } = string.Empty;
    public DateTime ConsumedAt { get; set; }
}
```

**Project Structure**:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  
  <ItemGroup>
    <!-- Only depends on Domain -->
    <ProjectReference Include="..\NutritionTracker.Domain\NutritionTracker.Domain.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="FluentValidation" Version="11.9.2" />
    <PackageReference Include="MediatR" Version="12.2.0" />
  </ItemGroup>
</Project>
```

---

### 3. Adapters Layer

#### A. Input Adapters (REST API)
**Location**: `src/Adapters/Input/NutritionTracker.RestApi`  
**Dependencies**: Application  
**Purpose**: Receives external HTTP requests and calls use cases

```csharp
[ApiController]
[Route("api/[controller]")]
public class NutritionController : ControllerBase
{
    private readonly ILogMealUseCase _logMeal;
    private readonly IGetDailyTotalsQuery _getDailyTotals;
    
    public NutritionController(
        ILogMealUseCase logMeal,
        IGetDailyTotalsQuery getDailyTotals)
    {
        _logMeal = logMeal;
        _getDailyTotals = getDailyTotals;
    }
    
    [HttpPost("log-meal")]
    public async Task<IActionResult> LogMeal([FromBody] LogMealRequest request)
    {
        var command = new LogMealCommand(
            request.UserId,
            request.FoodItemId,
            request.Quantity,
            request.MealType,
            DateTime.UtcNow
        );
        
        var result = await _logMeal.ExecuteAsync(command);
        return Ok(result);
    }
    
    [HttpGet("daily-totals")]
    public async Task<IActionResult> GetDailyTotals(
        [FromQuery] Guid userId,
        [FromQuery] DateTime date)
    {
        var query = new GetDailyTotalsQuery(userId, date);
        var result = await _getDailyTotals.ExecuteAsync(query);
        return Ok(result);
    }
}
```

**Project Structure**:
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\..\Core\NutritionTracker.Application\NutritionTracker.Application.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />
  </ItemGroup>
</Project>
```

#### B. Output Adapters (SQL Server)
**Location**: `src/Adapters/Output/NutritionTracker.SqlServer`  
**Dependencies**: Application  
**Purpose**: Implements data persistence

```csharp
// Implements the interface from Application layer
public class SqlServerUserRepository : IUserRepository
{
    private readonly NutritionDbContext _context;
    
    public SqlServerUserRepository(NutritionDbContext context)
    {
        _context = context;
    }
    
    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }
    
    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }
    
    public async Task<User> CreateAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }
}

// EF Core DbContext
public class NutritionDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<FoodItem> FoodItems => Set<FoodItem>();
    public DbSet<NutritionEntry> NutritionEntries => Set<NutritionEntry>();
    
    public NutritionDbContext(DbContextOptions<NutritionDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NutritionDbContext).Assembly);
    }
}
```

**Project Structure**:
```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="..\..\..\Core\NutritionTracker.Application\NutritionTracker.Application.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.0" />
  </ItemGroup>
</Project>
```

---

## Dependency Injection Setup

### Program.cs (Entry Point)
```csharp
var builder = WebApplication.CreateBuilder(args);

// ===== Register Application Services (Use Cases) =====
builder.Services.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();
builder.Services.AddScoped<ILogMealUseCase, LogMealUseCase>();
builder.Services.AddScoped<IGetDailyTotalsQuery, GetDailyTotalsQueryHandler>();

// ===== Register Infrastructure (Choose based on config) =====
var dbType = builder.Configuration["DatabaseType"];

if (dbType == "SqlServer")
{
    // SQL Server
    builder.Services.AddDbContext<NutritionDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection")));
    
    builder.Services.AddScoped<IUserRepository, SqlServerUserRepository>();
    builder.Services.AddScoped<IFoodRepository, SqlServerFoodRepository>();
    builder.Services.AddScoped<INutritionEntryRepository, SqlServerNutritionRepository>();
}
else if (dbType == "AzureTable")
{
    // Azure Table Storage
    builder.Services.Configure<AzureTableOptions>(
        builder.Configuration.GetSection("AzureTable"));
    
    builder.Services.AddScoped<IUserRepository, AzureTableUserRepository>();
    builder.Services.AddScoped<IFoodRepository, AzureTableFoodRepository>();
}
else if (dbType == "InMemory")
{
    // In-Memory (for testing)
    builder.Services.AddScoped<IUserRepository, InMemoryUserRepository>();
    builder.Services.AddScoped<IFoodRepository, InMemoryFoodRepository>();
}

// ===== Register External Services =====
builder.Services.Configure<SendGridOptions>(
    builder.Configuration.GetSection("SendGrid"));
builder.Services.AddScoped<IEmailSender, SendGridEmailSender>();

// ===== Add Framework Services =====
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

### Configuration Files

**appsettings.Development.json** (Development):
```json
{
  "DatabaseType": "InMemory",
  "Logging": {
    "LogLevel": {
      "Default": "Debug"
    }
  }
}
```

**appsettings.json** (Production):
```json
{
  "DatabaseType": "SqlServer",
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=NutritionTrackerDb;Trusted_Connection=true"
  },
  "SendGrid": {
    "ApiKey": "your-sendgrid-api-key",
    "FromEmail": "noreply@nutritiontracker.com"
  }
}
```

---

## Ports vs Adapters - Clarification

### Common Confusion: Where should Input/Output be?

There are two common approaches. Let's clarify which we're using:

#### ❌ Approach 2: Everything Under "Ports" Folder (Confusing)
```
src/
└── Ports/
    ├── Input/
    │   ├── Interfaces/      # IRegisterUserUseCase
    │   └── Adapters/        # REST API controllers
    └── Output/
        ├── Interfaces/      # IUserRepository
        └── Adapters/        # SQL Server implementations
```

**Problems**:
- Mixes interfaces and implementations in same folder
- Unclear project boundaries
- Less common in industry

#### ✅ Approach 1: Ports in Application, Adapters Separate (What We Use)
```
src/
├── Core/
│   └── Application/
│       └── Ports/           # 🔌 INTERFACES ONLY
│           ├── Input/       # IRegisterUserUseCase
│           └── Output/      # IUserRepository
│
└── Adapters/                # 🔌 IMPLEMENTATIONS ONLY
    ├── Input/               # REST API controllers
    └── Output/              # SQL Server repositories
```

**Benefits**:
- Clear separation: Interfaces in Application, Implementations outside
- Standard industry practice
- Better project organization
- Obvious dependency flow

### Terminology Clarification

| Term | Location | What It Is | Example |
|------|----------|------------|---------|
| **Input Port** | Application/Ports/Input | Interface (what app can do) | `ILogMealUseCase` |
| **Input Adapter** | Adapters/Input | Implementation (how to call app) | `NutritionController` |
| **Output Port** | Application/Ports/Output | Interface (what app needs) | `IUserRepository` |
| **Output Adapter** | Adapters/Output | Implementation (how app calls external) | `SqlServerUserRepository` |

### Visual: Ports vs Adapters

```
┌──────────────────────────────────────────────┐
│              APPLICATION                      │
│                                               │
│  ┌─────────────────────────────────────┐    │
│  │         INPUT PORT                   │    │
│  │  interface ILogMealUseCase           │    │
│  │  { ExecuteAsync(command); }          │    │
│  └──────────────▲──────────────────────┘    │
│                 │ implements                 │
│  ┌──────────────┴──────────────────────┐    │
│  │      OUTPUT PORT                     │    │
│  │  interface IUserRepository           │    │
│  │  { GetByIdAsync(id); }               │    │
│  └──────────────▲──────────────────────┘    │
└────────────────│├──────────────────────────┘
                 ││ implements
┌────────────────▼┤
│  INPUT ADAPTER  │                      
│  (REST Controller)                    
│  calls ILogMealUseCase                
└─────────────────┘                      
                                         
                 ┌─────────────────────┐
                 │  OUTPUT ADAPTER     │
                 │  (SQL Repository)   │
                 │  implements         │
                 │  IUserRepository    │
                 └─────────────────────┘
```

**Key Takeaway**:
- **Ports** = Contracts (interfaces) defined in Application
- **Adapters** = Concrete implementations outside Application
- Input/Output distinction exists in BOTH ports and adapters

---

## Complete Folder Structure

```
DailyNutritionCaloriesTracker/
│
├── src/
│   ├── Core/                                    
│   │   ├── NutritionTracker.Domain/             # Pure business logic (NO dependencies)
│   │   │   ├── Entities/
│   │   │   │   ├── EntityBase.cs
│   │   │   │   ├── User.cs
│   │   │   │   ├── FoodItem.cs
│   │   │   │   ├── NutritionEntry.cs
│   │   │   │   └── Recipe.cs
│   │   │   ├── DomainServices/
│   │   │   │   ├── CalorieCalculationService.cs
│   │   │   │   └── MacroCalculationService.cs
│   │   │   ├── DomainEvents/
│   │   │   │   ├── MealLoggedEvent.cs
│   │   │   │   └── GoalAchievedEvent.cs
│   │   │   ├── Exceptions/
│   │   │   │   ├── DomainException.cs
│   │   │   │   ├── UserNotFoundException.cs
│   │   │   │   └── InvalidNutritionDataException.cs
│   │   │   └── NutritionTracker.Domain.csproj
│   │   │
│   │   └── NutritionTracker.Application/        # Use cases & interfaces (depends on Domain only)
│   │       ├── Ports/
│   │       │   ├── Input/                       # 🔌 What app CAN DO (use case interfaces)
│   │       │   │   ├── Users/
│   │       │   │   │   ├── IRegisterUserUseCase.cs
│   │       │   │   │   └── IGetUserProfileQuery.cs
│   │       │   │   ├── Nutrition/
│   │       │   │   │   ├── ILogMealUseCase.cs
│   │       │   │   │   └── IGetDailyTotalsQuery.cs
│   │       │   │   ├── Foods/
│   │       │   │   │   └── ISearchFoodsQuery.cs
│   │       │   │   └── Recipes/
│   │       │   │       └── ICreateRecipeUseCase.cs
│   │       │   │
│   │       │   └── Output/                      # 🔌 What app NEEDS (dependency interfaces)
│   │       │       ├── Repositories/
│   │       │       │   ├── IUserRepository.cs
│   │       │       │   ├── IFoodRepository.cs
│   │       │       │   └── INutritionEntryRepository.cs
│   │       │       └── ExternalServices/
│   │       │           ├── IEmailSender.cs
│   │       │           └── IFileStorage.cs
│   │       │
│   │       ├── UseCases/                        # Business logic implementations
│   │       │   ├── Users/
│   │       │   │   └── RegisterUserUseCase.cs
│   │       │   ├── Nutrition/
│   │       │   │   ├── LogMealUseCase.cs
│   │       │   │   └── GetDailyTotalsQueryHandler.cs
│   │       │   └── Foods/
│   │       │       └── SearchFoodsQueryHandler.cs
│   │       │
│   │       ├── Commands/                        # Write operations (CQRS)
│   │       │   ├── RegisterUserCommand.cs
│   │       │   └── LogMealCommand.cs
│   │       │
│   │       ├── Queries/                         # Read operations (CQRS)
│   │       │   ├── GetUserProfileQuery.cs
│   │       │   └── GetDailyTotalsQuery.cs
│   │       │
│   │       ├── DTOs/                            # Data transfer objects
│   │       │   ├── UserProfileDto.cs
│   │       │   ├── NutritionEntryDto.cs
│   │       │   └── DailyTotalsDto.cs
│   │       │
│   │       ├── Validators/                      # FluentValidation
│   │       │   ├── RegisterUserCommandValidator.cs
│   │       │   └── LogMealCommandValidator.cs
│   │       │
│   │       └── NutritionTracker.Application.csproj
│   │
│   ├── Adapters/                                
│   │   ├── Input/                               # 🔌 How external world CALLS app
│   │   │   └── NutritionTracker.RestApi/
│   │   │       ├── Controllers/
│   │   │       │   ├── UsersController.cs
│   │   │       │   ├── NutritionController.cs
│   │   │       │   └── FoodsController.cs
│   │   │       ├── Middleware/
│   │   │       │   ├── ErrorHandlingMiddleware.cs
│   │   │       │   └── AuthenticationMiddleware.cs
│   │   │       ├── Models/
│   │   │       │   ├── Requests/
│   │   │       │   │   ├── RegisterUserRequest.cs
│   │   │       │   │   └── LogMealRequest.cs
│   │   │       │   └── Responses/
│   │   │       │       ├── UserProfileResponse.cs
│   │   │       │       └── NutritionEntryResponse.cs
│   │   │       ├── Program.cs                   # 🔧 DI Configuration here
│   │   │       ├── appsettings.json
│   │   │       ├── appsettings.Development.json
│   │   │       └── NutritionTracker.RestApi.csproj
│   │   │
│   │   └── Output/                              # 🔌 How app CALLS external world
│   │       ├── NutritionTracker.SqlServer/
│   │       │   ├── Repositories/
│   │       │   │   ├── SqlServerUserRepository.cs
│   │       │   │   ├── SqlServerFoodRepository.cs
│   │       │   │   └── SqlServerNutritionRepository.cs
│   │       │   ├── DbContext/
│   │       │   │   └── NutritionDbContext.cs
│   │       │   ├── EntityConfigurations/
│   │       │   │   ├── UserConfiguration.cs
│   │       │   │   └── FoodItemConfiguration.cs
│   │       │   ├── Migrations/
│   │       │   └── NutritionTracker.SqlServer.csproj
│   │       │
│   │       ├── NutritionTracker.AzureTable/     # Alternative NoSQL adapter
│   │       │   ├── Repositories/
│   │       │   │   ├── AzureTableUserRepository.cs
│   │       │   │   └── AzureTableFoodRepository.cs
│   │       │   └── NutritionTracker.AzureTable.csproj
│   │       │
│   │       └── NutritionTracker.ExternalServices/ # Third-party integrations
│   │           ├── Email/
│   │           │   └── SendGridEmailSender.cs
│   │           ├── FileStorage/
│   │           │   └── AzureBlobFileStorage.cs
│   │           └── NutritionTracker.ExternalServices.csproj
│   │
│   └── Presentation/
│       └── NutritionTracker.Web/                # Frontend (Vue.js)
│           ├── src/
│           │   ├── components/
│           │   │   ├── UserProfile.vue
│           │   │   └── MealLogger.vue
│           │   ├── services/
│           │   │   └── api.js                   # API client
│           │   ├── views/
│           │   │   ├── Home.vue
│           │   │   └── Dashboard.vue
│           │   ├── App.vue
│           │   └── main.js
│           ├── public/
│           ├── package.json
│           └── vite.config.js
│
├── tests/
│   ├── NutritionTracker.Domain.Tests/
│   │   ├── Entities/
│   │   └── DomainServices/
│   ├── NutritionTracker.Application.Tests/
│   │   ├── UseCases/
│   │   └── Validators/
│   └── NutritionTracker.Integration.Tests/
│       ├── Api/
│       └── Database/
│
├── docs/
│   ├── Hexagonal Architecture Implementation.md  # This document
│   └── user-id-bug-fix.md
│
├── .gitignore
├── README.md
└── DailyNutritionCaloriesTracker.sln
```

### Folder Purpose Summary

| Folder | Purpose | Examples | When to Create? |
|--------|---------|----------|-----------------|
| **Domain/Entities/** | Business objects with identity | `User`, `FoodItem`, `NutritionEntry` | ✅ Start here - core business objects |
| **Domain/DomainServices/** | Pure business calculations | `CalorieCalculationService` | When calculations don't belong in entities |
| **Domain/DomainEvents/** | Domain-level events | `MealLoggedEvent` | When you need event-driven patterns |
| **Domain/Exceptions/** | Business rule violations | `UserNotFoundException` | When you need domain-specific errors |
| **Application/Ports/Input/** | Use case interfaces | `ILogMealUseCase` | Define what app can do |
| **Application/Ports/Output/** | Dependency interfaces | `IUserRepository` | Define what app needs from outside |
| **Application/UseCases/** | Business logic orchestration | `LogMealUseCase` | Implement actual workflows |
| **Application/Commands/** | Write operation DTOs | `LogMealCommand` | Commands that change state |
| **Application/Queries/** | Read operation DTOs | `GetDailyTotalsQuery` | Queries that read data |
| **Application/DTOs/** | Data transfer objects | `NutritionEntryDto` | Return data to clients |
| **Adapters/Input/RestApi/** | HTTP controllers | `NutritionController` | Handle HTTP requests |
| **Adapters/Output/SqlServer/** | Database repositories | `SqlServerUserRepository` | Persist data to SQL Server |
| **Adapters/Output/ExternalServices/** | Third-party integrations | `SendGridEmailSender` | Call external APIs |

### ⏭️ Future: When to Add ValueObjects Folder

**Add `Domain/ValueObjects/` folder when you need to extract these concepts:**
```csharp
// Example 1: Email validation (when you see validation repeated)
public record Email(string Value)
{
    public Email(string value) : this(ValidateEmail(value)) { }
    private static string ValidateEmail(string email)
    {
        if (!email.Contains("@")) throw new InvalidEmailException();
        return email.ToLowerInvariant();
    }
}

// Example 2: Macro calculations (when logic is scattered)
public record MacroNutrients(double Protein, double Carbs, double Fat)
{
    public double TotalCalories => (Protein * 4) + (Carbs * 4) + (Fat * 9);
    public MacroNutrients Add(MacroNutrients other) => 
        new(Protein + other.Protein, Carbs + other.Carbs, Fat + other.Fat);
}

// Example 3: Serving sizes (when you need validation + formatting)
public record ServingSize(double Amount, string Unit)
{
    public ServingSize(double amount, string unit) : this(amount, unit.ToLower())
    {
        if (amount <= 0) throw new InvalidServingSizeException();
    }
    public override string ToString() => $"{Amount} {Unit}";
}
```

**Signs you need Value Objects:**
- ✅ Same validation logic repeated in multiple entities
- ✅ Primitive obsession (using strings/doubles for complex concepts)
- ✅ Calculations that don't belong to any specific entity
- ✅ Need for immutable, value-based equality

**Until then:** Keep it simple with entities and add ValueObjects only when you see these patterns emerge!