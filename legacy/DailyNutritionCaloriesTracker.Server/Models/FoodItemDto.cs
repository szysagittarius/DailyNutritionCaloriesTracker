namespace NutritionTracker.Api.Models;

public class FoodItemDto : DtoBase
{
    public Guid FoodNutritionId { get; set; }
    public int Unit { get; set; }
    public Guid FoodLogId { get; set; }


}
