using Azure;
using Azure.Data.Tables;

namespace NutritionTracker.AzureTableStorage.Entities;

/// <summary>
/// FoodNutrition entity for Azure Table Storage
/// PartitionKey: "FOOD" (all food items in same partition)
/// RowKey: FoodId (Guid)
/// </summary>
public class FoodNutritionTableEntity : ITableEntity
{
    public string PartitionKey { get; set; } = "FOOD";
    public string RowKey { get; set; } = string.Empty; // FoodId
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    // FoodNutrition properties
    public string Name { get; set; } = string.Empty;
    public string Measurement { get; set; } = string.Empty;
    public double Carbs { get; set; }
    public double Fat { get; set; }
    public double Protein { get; set; }
    public double Calories { get; set; }
}
