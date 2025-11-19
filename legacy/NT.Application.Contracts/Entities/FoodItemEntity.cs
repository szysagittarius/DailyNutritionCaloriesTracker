namespace NT.Application.Contracts.Entities;
public class FoodItemEntity : EntityBase
{
    public Guid FoodNutritionId { get; set; }
    public int Unit { get; set; }
    public Guid FoodLogId { get; set; }

    // Add navigation property to FoodNutrition
    public FoodNutritionEntity FoodNutrition { get; set; }

    // Additional methods to manipulate and query the food item
}
