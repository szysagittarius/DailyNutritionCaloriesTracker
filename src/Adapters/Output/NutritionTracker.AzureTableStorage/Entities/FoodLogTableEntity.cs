using Azure;
using Azure.Data.Tables;

namespace NutritionTracker.AzureTableStorage.Entities;

/// <summary>
/// FoodLog entity for Azure Table Storage
/// PartitionKey: UserId (all logs for a user together - efficient for GetByUserId queries)
/// RowKey: DateTime_LogId (sortable by date, unique with LogId)
/// </summary>
public class FoodLogTableEntity : ITableEntity
{
    public string PartitionKey { get; set; } = string.Empty; // UserId
    public string RowKey { get; set; } = string.Empty; // DateTime_LogId for sorting
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    // FoodLog properties
    public string Id { get; set; } = string.Empty;
    public DateTime DateTime { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime UpdateTime { get; set; }
    public string UserId { get; set; } = string.Empty;
    public double TotalCalories { get; set; }
    public double TotalCarbs { get; set; }
    public double TotalProtein { get; set; }
    public double TotalFat { get; set; }
    
    // FoodItems stored as JSON string (denormalized for performance)
    public string FoodItemsJson { get; set; } = string.Empty;
}
