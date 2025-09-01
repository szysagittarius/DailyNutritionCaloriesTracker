# Feature Implementation: User-Specific Food Log Retrieval with Nutritional Calculations

## Overview
This document details the implementation of a feature that retrieves food log entries for a specific user and calculates total nutritional values (calories, carbs, protein, fat) from individual food items in each log.

## Feature Requirements
- Create API endpoint to retrieve food logs for a specific user
- Calculate total nutritional values from food items within each log
- Handle navigation properties properly in Entity Framework
- Maintain Hexagonal Architecture principles

## Issues Encountered and Solutions

### 1. Missing Navigation Properties in Entity Framework

#### **Issue**
Food logs were returned with empty `foodItems` arrays, preventing proper nutritional calculations.

#### **Root Cause**
Entity Framework wasn't including related `FoodItems` and `FoodNutrition` when querying `FoodLog` entities.

#### **Solution**
Updated the repository layer to include navigation properties using Entity Framework's `.Include()` and `.ThenInclude()` methods.

**Code Changes:**

```csharp
// filepath: NT.Repositories\Implementations\Repositories\FoodLogRepository.cs
using Microsoft.EntityFrameworkCore;
using NT.Database.Context;
using NT.Database.Entities;
using NT.Ef.Repositories.Abstractions;

namespace NT.Ef.Repositories.Implementations.Repositories;
internal class FoodLogRepository : BaseRepository<FoodLog>, IFoodLogRepository
{
    public FoodLogRepository(NutritionTrackerDbContext dbContext) : base(dbContext)
    {
    }

    // Override the GetAll method to include FoodItems AND their FoodNutrition
    public new IQueryable<FoodLog> GetAll()
    {
        return dbContext.Set<FoodLog>()
            .Include(fl => fl.FoodItems)
                .ThenInclude(fi => fi.FoodNutrition); // Include the FoodNutrition for each FoodItem
    }

    // Override GetByIdAsync to include FoodItems and their FoodNutrition
    public new async Task<FoodLog> GetByIdAsync(Guid id)
    {
        return await dbContext.Set<FoodLog>()
            .Include(fl => fl.FoodItems)
                .ThenInclude(fi => fi.FoodNutrition)
            .FirstOrDefaultAsync(fl => fl.Id == id);
    }
}
```

### 2. Entity Framework Collection Materialization Issues

#### **Issue**
Runtime error: `System.Linq.ThrowHelper.ThrowNotSupportedException()` when Entity Framework tried to populate navigation properties.

#### **Root Cause**
Entities used `IEnumerable<T>` (read-only) instead of `ICollection<T>` (mutable) for navigation properties, and used `record` types with `init` properties.

#### **Solution**
Converted all database entities from `record` types to `class` types with proper mutable collections.

**Code Changes:**

```csharp
// filepath: DNCT.Database\Entities\FoodLog.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NT.Database.Entities;

[Table("FoodLogs")]
public class FoodLog  // Changed from 'record' to 'class'
{
    [Key]
    [Column("Id", TypeName = "uniqueidentifier")]
    public Guid Id { get; set; }  // Changed from 'required' and 'init'

    [Column("DateTime", TypeName = "datetime")]
    public DateTime DateTime { get; set; }  // Changed from 'required' and 'init'

    [Column("CreateTime", TypeName = "datetime")]
    public DateTime CreateTime { get; set; }  // Changed from 'required' and 'init'

    [Column("UpdateTime", TypeName = "datetime")]
    public DateTime UpdateTime { get; set; }  // Changed from 'required' and 'init'

    [ForeignKey("User")]
    [Column("UserId", TypeName = "uniqueidentifier")]
    public Guid UserId { get; set; }  // Changed from 'required' and 'init'

    // Navigation properties - Changed to ICollection and made nullable/virtual
    public virtual User? User { get; set; }
    public virtual ICollection<FoodItem> FoodItems { get; set; } = new List<FoodItem>();

    // Added calculated properties for totals
    [Column("TotalCalories", TypeName = "float")]
    public double TotalCalories { get; set; }

    [Column("TotalCarbs", TypeName = "float")]
    public double TotalCarbs { get; set; }

    [Column("TotalProtein", TypeName = "float")]
    public double TotalProtein { get; set; }

    [Column("TotalFat", TypeName = "float")]
    public double TotalFat { get; set; }
}
```

```csharp
// filepath: DNCT.Database\Entities\FoodItem.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NT.Database.Entities;

[Table("FoodItems")]
public class FoodItem  // Changed from 'record' to 'class'
{
    [Key]
    [Column("Id", TypeName = "uniqueidentifier")]
    public Guid Id { get; set; }  // Changed from 'required' and 'init'

    [ForeignKey("FoodNutrition")]
    [Column("FoodNutritionId", TypeName = "uniqueidentifier")]
    public Guid FoodNutritionId { get; set; }  // Changed from 'required' and 'init'

    public virtual FoodNutrition? FoodNutrition { get; set; }  // Made nullable and virtual

    [Column("Unit", TypeName = "int")]
    public int Unit { get; set; }  // Changed from 'required' and 'init'

    [ForeignKey("FoodLog")]
    [Column("FoodLogId", TypeName = "uniqueidentifier")]
    public Guid FoodLogId { get; set; }  // Changed from 'required' and 'init'

    public virtual FoodLog? FoodLog { get; set; }  // Made nullable and virtual
}
```

```csharp
// filepath: DNCT.Database\Entities\FoodNutrition.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NT.Database.Entities;

[Table("FoodNutritions")]
public class FoodNutrition  // Changed from 'record class' to 'class'
{
    [Key]
    [Column("Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }  // Changed from 'required' and 'init'

    [Column("Name")]
    public string Name { get; set; } = string.Empty;  // Changed from 'required' and 'init'

    [Column("Measurement")]
    public string Measurement { get; set; } = string.Empty;  // Changed from 'required' and 'init'

    [Column("Carbs")]
    public double Carbs { get; set; }  // Changed from 'required' and 'init'

    [Column("Fat")]
    public double Fat { get; set; }  // Changed from 'required' and 'init'

    [Column("Protein")]
    public double Protein { get; set; }  // Changed from 'required' and 'init'

    [Column("Calories")]
    public double Calories { get; set; }  // Changed from 'required' and 'init'

    // Navigation property
    public virtual ICollection<FoodItem> FoodItems { get; set; } = new List<FoodItem>();
}
```

```csharp
// filepath: NT.Application.Contracts\Entities\FoodLogEntity.cs
namespace NT.Application.Contracts.Entities;
public class FoodLogEntity
{
    public Guid Id { get; set; } // Added - Required for Entity Framework
    public double TotalCalories { get; set; }
    public double TotalCarbs { get; set; }
    public double TotalProtein { get; set; }
    public double TotalFat { get; set; }

    public Guid UserId { get; set; }

    public DateTime DateTime { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime UpdateTime { get; set; }
    
    // Changed from IEnumerable to ICollection for EF compatibility
    public ICollection<FoodItemEntity> FoodItems { get; set; } = new List<FoodItemEntity>();
}
```

### 3. Nutritional Calculation Logic

#### **Issue**
Controller tried to access `item.Calories` directly, but `FoodItemEntity` only contains `FoodNutritionId` (foreign key).

#### **Root Cause**
Nutritional values are stored in the related `FoodNutritionEntity`, not directly in `FoodItemEntity`.

#### **Solution**
Implemented calculation logic using navigation properties and unit scaling.

**Code Changes:**

```csharp
// filepath: DailyNutritionCaloriesTracker.Server\Controllers\FoodLogController.cs
[HttpGet("GetUserFoodLogs/{userId}")]
public async Task<ActionResult<IEnumerable<FoodLogDto>>> GetUserFoodLogs(Guid userId)
{
    try
    {
        if (userId == Guid.Empty)
        {
            return BadRequest("Valid user ID is required");
        }

        MapperConfiguration dtoMapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new FoodNutritionDtoProfiler());
            cfg.AddProfile(new FoodItemDtoProfiler());
            cfg.AddProfile(new FoodLogDtoProfiler());
            cfg.AddProfile(new UserDtoProfiler());
        });

        IMapper dtoMapper = dtoMapperConfig.CreateMapper();

        // Get food logs for the specific user - FoodItems will now be included
        IEnumerable<FoodLogEntity> entities = await _foodLogService.GetAllAsync();
        
        // Filter by user ID
        var userEntities = entities.Where(e => e.UserId == userId);

        IEnumerable<FoodLogDto> foodLogDtos = userEntities.Select(entity => {
            var dto = dtoMapper.Map<FoodLogDto>(entity);
            
            // Calculate totals from FoodItems using their FoodNutrition and Unit
            if (entity.FoodItems != null && entity.FoodItems.Any())
            {
                // Formula: (nutrition_per_100g * unit_in_grams) / 100
                dto.TotalCalories = entity.FoodItems.Sum(item => 
                    item.FoodNutrition != null ? (item.FoodNutrition.Calories * item.Unit / 100.0) : 0);
                
                dto.TotalCarbs = entity.FoodItems.Sum(item => 
                    item.FoodNutrition != null ? (item.FoodNutrition.Carbs * item.Unit / 100.0) : 0);
                
                dto.TotalProtein = entity.FoodItems.Sum(item => 
                    item.FoodNutrition != null ? (item.FoodNutrition.Protein * item.Unit / 100.0) : 0);
                
                dto.TotalFat = entity.FoodItems.Sum(item => 
                    item.FoodNutrition != null ? (item.FoodNutrition.Fat * item.Unit / 100.0) : 0);
                
                _logger.LogInformation("Calculated totals for FoodLog - Calories: {Calories}, Carbs: {Carbs}", 
                    dto.TotalCalories, dto.TotalCarbs);
            }
            else
            {
                _logger.LogWarning("FoodLog has no FoodItems - keeping existing totals");
            }
            
            return dto;
        });

        _logger.LogInformation("Retrieved {Count} food log entries for user {UserId}", foodLogDtos.Count(), userId);

        return Ok(foodLogDtos);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error occurred while fetching food logs for user {UserId}", userId);
        return StatusCode(500, "An error occurred while fetching food logs");
    }
}
```

### 4. Database Schema Updates

#### **Issue**
Added new properties to entities that needed to be reflected in the database schema.

#### **Solution**
Created Entity Framework migration to add new columns.

**Migration Commands:**
```bash
# From DailyNutritionCaloriesTracker.Server folder:
dotnet ef migrations add AddTotalNutritionToFoodLog --project ..\DNCT.Database --startup-project .
dotnet ef database update --project ..\DNCT.Database --startup-project .
```

**Generated Migration:**
```csharp
public partial class AddTotalNutritionToFoodLog : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<double>(
            name: "TotalCalories", table: "FoodLogs", type: "float", nullable: false, defaultValue: 0.0);
        migrationBuilder.AddColumn<double>(
            name: "TotalCarbs", table: "FoodLogs", type: "float", nullable: false, defaultValue: 0.0);
        migrationBuilder.AddColumn<double>(
            name: "TotalFat", table: "FoodLogs", type: "float", nullable: false, defaultValue: 0.0);
        migrationBuilder.AddColumn<double>(
            name: "TotalProtein", table: "FoodLogs", type: "float", nullable: false, defaultValue: 0.0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(name: "TotalCalories", table: "FoodLogs");
        migrationBuilder.DropColumn(name: "TotalCarbs", table: "FoodLogs");
        migrationBuilder.DropColumn(name: "TotalFat", table: "FoodLogs");
        migrationBuilder.DropColumn(name: "TotalProtein", table: "FoodLogs");
    }
}
```

### 5. Frontend Integration

#### **Code Changes:**
```vue
// filepath: dailynutritioncaloriestracker.client\src\components\FoodLogPage.vue
<script setup>
import { ref, computed, onMounted } from 'vue'
import api from '../services/api'

// Get current user from API service
const currentUser = ref(api.getCurrentUser())
const userId = currentUser.value?.id || '00000000-0000-0000-0000-000000000001'

const fetchFoodLogData = async () => {
  isLoading.value = true
  try {
    console.log('Fetching food log data for user:', userId)
    
    // Call the new user-specific endpoint
    const response = await fetch(`foodlog/GetUserFoodLogs/${userId}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json'
      }
    })
    
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`)
    }
    
    const data = await response.json()
    
    // Transform the data - handle both cases: with and without food items
    const transformedData = []
    if (Array.isArray(data)) {
      data.forEach(log => {
        if (log.foodItems && Array.isArray(log.foodItems) && log.foodItems.length > 0) {
          // If there are food items, create a row for each
          log.foodItems.forEach(item => {
            transformedData.push({
              id: `${log.id || 'unknown'}-${item.id || Math.random()}`,
              dateLogged: log.dateTime || log.createTime,
              foodName: item.foodName || 'Unknown Food',
              quantity: item.quantity || 0,
              calories: item.calories || 0,
              protein: item.protein || 0,
              carbs: item.carbs || 0,
              fat: item.fat || 0
            })
          })
        } else {
          // If no food items, show the log entry with totals
          transformedData.push({
            id: log.id || Math.random(),
            dateLogged: log.dateTime || log.createTime,
            foodName: 'Food Log Entry',
            quantity: '-',
            calories: log.totalCalories || 0,
            protein: log.totalProtein || 0,
            carbs: log.totalCarbs || 0,
            fat: log.totalFat || 0
          })
        }
      })
    }
    
    transformedData.sort((a, b) => new Date(b.dateLogged) - new Date(a.dateLogged))
    foodLogItems.value = transformedData
    
  } catch (error) {
    console.error('Error fetching food log data:', error)
    alert('Failed to load food log data: ' + error.message)
  } finally {
    isLoading.value = false
  }
}
</script>
```

## Architecture Flow

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Frontend      │───▶│   Controller    │───▶│    Service      │───▶│  Data Handler   │
│  (Vue.js)       │    │ (FoodLogCtrl)   │    │ (FoodLogSvc)    │    │ (FoodLogDH)     │
└─────────────────┘    └─────────────────┘    └─────────────────┘    └─────────────────┘
                                                                               │
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│    Database     │◀───│   Repository    │◀───│     Mapper      │◀───│   Repository    │
│   (SQL Server)  │    │ (FoodLogRepo)   │    │   (AutoMapper)  │    │ (FoodLogRepo)   │
└─────────────────┘    └─────────────────┘    └─────────────────┘    └─────────────────┘
```

## Key Technical Decisions

### 1. **Calculation Location**
- **Decision**: Calculate totals in the controller layer
- **Rationale**: Keeps business logic close to API layer, allows for real-time calculation
- **Alternative**: Store calculated values in database (implemented as backup via migration)

### 2. **Navigation Property Loading**
- **Decision**: Use `.Include()` and `.ThenInclude()` in repository
- **Rationale**: Ensures related data is loaded efficiently in single query
- **Alternative**: Lazy loading (not suitable for API scenarios)

### 3. **Entity Design**
- **Decision**: Convert from `record` to `class` types
- **Rationale**: Better Entity Framework compatibility and change tracking
- **Alternative**: Keep records with custom configurations (more complex)

## Performance Considerations

1. **Database Queries**: Single query with joins via `.Include()` is more efficient than multiple queries
2. **Memory Usage**: Loading navigation properties increases memory usage but reduces database round trips
3. **Calculation**: Real-time calculation provides accuracy but adds CPU overhead

## Testing Strategy

1. **Unit Tests**: Test calculation logic with mock data
2. **Integration Tests**: Test complete flow from repository to controller
3. **Database Tests**: Verify navigation properties are loaded correctly
4. **API Tests**: Test endpoint with various user scenarios

## Future Improvements

1. **Caching**: Implement caching for frequently accessed food logs
2. **Pagination**: Add pagination support for large datasets
3. **Background Calculation**: Move total calculations to background service
4. **Performance Monitoring**: Add metrics for query performance

## Conclusion

This feature successfully implements user-specific food log retrieval with proper nutritional calculations while maintaining the existing Hexagonal Architecture. The solution addresses Entity Framework navigation property challenges and provides a robust foundation for future nutrition tracking features.