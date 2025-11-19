# Input Adapters - Shared Contracts Library

## Overview

The `NutritionTracker.Api.Contracts` library contains **shared code** that is reused across **all input adapters** (REST API, Azure Functions, gRPC, GraphQL, etc.). This follows the **DRY principle** and ensures consistency across different entry points.

## Purpose

Input adapters need similar functionality:
- ✅ Request/Response models for HTTP communication
- ✅ Error handling and exception mapping
- ✅ Common API response wrappers
- ✅ Validation models
- ✅ Pagination helpers

Instead of duplicating this code in each adapter, we centralize it here.

## Project Structure

```
NutritionTracker.Api.Contracts/
├── Common/
│   ├── ApiResponse.cs          # Standard API response wrapper
│   ├── ErrorResponse.cs        # Error response model
│   └── PaginationModels.cs     # Pagination request/response
├── Requests/
│   └── FoodLogRequests.cs      # Request models with ToCommand() mapping
├── Responses/
│   └── FoodLogResponses.cs     # Response models with FromDto() mapping
├── Exceptions/
│   └── ApiExceptions.cs        # Custom API exceptions (400, 404, 401, etc.)
└── Extensions/
    └── ExceptionExtensions.cs  # Exception helper methods
```

## Key Concepts

### 1. **Separation of Concerns**

```
HTTP Request → Request Model → Application Command → Use Case
Use Case → Application DTO → Response Model → HTTP Response
```

**Why?**
- Application layer remains HTTP-agnostic
- Request/Response models can have HTTP-specific attributes (validation, binding)
- Easy to add different adapters (gRPC, GraphQL) with their own models

### 2. **Request Models**

Located in `Requests/` folder. Responsible for:
- Accepting HTTP input
- Validation attributes (DataAnnotations, FluentValidation)
- Converting to Application **Commands/Queries**

Example:
```csharp
public class CreateFoodLogRequest
{
    public DateTime DateTime { get; set; }
    public Guid UserId { get; set; }
    public List<FoodItemRequest> FoodItems { get; set; } = new();

    // Maps to Application Command
    public CreateFoodLogCommand ToCommand()
    {
        return new CreateFoodLogCommand(DateTime, UserId, 
            FoodItems.Select(fi => new FoodItemDto(fi.FoodNutritionId, fi.Unit)).ToList());
    }
}
```

### 3. **Response Models**

Located in `Responses/` folder. Responsible for:
- HTTP-specific response structure
- Converting from Application **DTOs**
- Adding metadata (timestamps, links, etc.)

Example:
```csharp
public class FoodLogResponse
{
    public Guid Id { get; set; }
    public DateTime DateTime { get; set; }
    // ...
    
    // Maps from Application DTO
    public static FoodLogResponse FromDto(FoodLogDto dto)
    {
        return new FoodLogResponse { /* mapping */ };
    }
}
```

### 4. **API Response Wrapper**

Provides consistent response structure:

```csharp
// Success response
ApiResponse<FoodLogResponse>.SuccessResult(data, "Food log created");

// Error response
ApiResponse<FoodLogResponse>.FailureResult("Validation failed", errors);
```

**Response structure:**
```json
{
  "success": true,
  "data": { /* actual data */ },
  "message": "Operation succeeded",
  "errors": []
}
```

### 5. **Custom Exceptions**

Standardized exceptions with HTTP status codes:

```csharp
throw new NotFoundException("FoodLog", id);  // 404
throw new ValidationException("Invalid input", errors);  // 400
throw new UnauthorizedException();  // 401
throw new ForbiddenException();  // 403
throw new BusinessRuleException("Cannot delete active log");  // 409
```

### 6. **Exception Extensions**

Helper methods for exception handling:

```csharp
// Get appropriate HTTP status code
int statusCode = exception.GetStatusCode();  // 404, 400, 500, etc.

// Convert to ErrorResponse
ErrorResponse error = exception.ToErrorResponse(includeStackTrace: false);
```

## Usage in Controllers

### Before (Without Contracts)
```csharp
[HttpPost]
public async Task<ActionResult<FoodLogDto>> Create(CreateFoodLogCommand command)
{
    try
    {
        var result = await _useCase.ExecuteAsync(command);
        return Ok(result);  // Exposes internal DTO directly
    }
    catch (Exception ex)
    {
        return StatusCode(500, ex.Message);  // No structured error handling
    }
}
```

### After (With Contracts)
```csharp
[HttpPost]
[ProducesResponseType(typeof(ApiResponse<FoodLogResponse>), 201)]
[ProducesResponseType(typeof(ErrorResponse), 400)]
public async Task<IActionResult> Create(CreateFoodLogRequest request)
{
    try
    {
        var command = request.ToCommand();  // Map to Application layer
        var dto = await _useCase.ExecuteAsync(command);
        var response = FoodLogResponse.FromDto(dto);  // Map to HTTP layer
        
        return CreatedAtAction(nameof(GetByUser), 
            new { userId = response.UserId }, 
            ApiResponse<FoodLogResponse>.SuccessResult(response));
    }
    catch (Exception ex)
    {
        return StatusCode(ex.GetStatusCode(), ex.ToErrorResponse());
    }
}
```

## Benefits

### ✅ **Reusability**
- REST API, Azure Functions, gRPC all use the same contracts
- No code duplication

### ✅ **Consistency**
- All input adapters return the same response structure
- Unified error handling

### ✅ **Maintainability**
- Change once, applies everywhere
- Easier to add new adapters

### ✅ **Testability**
- Shared models can be tested once
- Easier to mock

### ✅ **Separation of Concerns**
- Application layer stays HTTP-agnostic
- Input layer handles HTTP-specific concerns

## When to Use This Library

### ✅ **Use for:**
- Request/Response models
- API response wrappers
- Error handling
- Pagination
- Common validation logic
- API versioning models

### ❌ **Don't use for:**
- Business logic (belongs in Application/Domain)
- Database concerns (belongs in Output adapters)
- Framework-specific code (belongs in specific adapter projects)

## Example: Azure Functions Adapter

When you create an Azure Functions adapter, you'll reference this same library:

```csharp
// Azure Function
[FunctionName("CreateFoodLog")]
public async Task<IActionResult> Run(
    [HttpTrigger(AuthorizationLevel.Function, "post")] 
    CreateFoodLogRequest request,  // Same request model!
    ILogger log)
{
    try
    {
        var command = request.ToCommand();
        var dto = await _useCase.ExecuteAsync(command);
        var response = FoodLogResponse.FromDto(dto);  // Same response model!
        
        return new OkObjectResult(ApiResponse<FoodLogResponse>.SuccessResult(response));
    }
    catch (Exception ex)
    {
        return new ObjectResult(ex.ToErrorResponse())  // Same error handling!
        {
            StatusCode = ex.GetStatusCode()
        };
    }
}
```

## Dependencies

```
NutritionTracker.Api.Contracts
    └── NutritionTracker.Application
            └── NutritionTracker.Domain
```

**Important:** This library only depends on the **Application layer**, not on ASP.NET Core or any specific framework.

## Adding New Shared Code

When adding new features, ask yourself:
1. **Will this be used by multiple input adapters?** → Add to Contracts
2. **Is this HTTP/API-specific?** → Add to Contracts
3. **Is this framework-specific?** → Keep in specific adapter (RestApi, AzureFunctions)

### Examples:
- **Shared:** Request/Response models, error handling, API wrappers
- **Not shared:** ASP.NET Core middleware, Azure Functions bindings, framework configurations

## Summary

The `NutritionTracker.Api.Contracts` library is the **common foundation** for all input adapters, providing:
- 🔄 Request → Command mapping
- 🔄 DTO → Response mapping  
- ⚠️ Consistent error handling
- 📦 Standard API responses
- 📄 Pagination support

This design ensures that whether you access the system via REST API, Azure Functions, or any future adapter, you get the **same consistent experience**.
