namespace NutritionTracker.AzureTableStorage.Entities;

/// <summary>
/// Helper class for serializing FoodItems to JSON
/// </summary>
public class FoodItemData
{
    public string Id { get; set; } = string.Empty;
    public string FoodNutritionId { get; set; } = string.Empty;
    public int Unit { get; set; }
    public string FoodLogId { get; set; } = string.Empty;
    
    // Denormalized FoodNutrition data for performance
    public string FoodName { get; set; } = string.Empty;
    public string Measurement { get; set; } = string.Empty;
    public double Carbs { get; set; }
    public double Fat { get; set; }
    public double Protein { get; set; }
    public double Calories { get; set; }
}
