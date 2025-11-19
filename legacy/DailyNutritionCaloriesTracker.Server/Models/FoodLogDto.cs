namespace NutritionTracker.Api.Models;

public class FoodLogDto
{
    public double TotalCalories { get; set; }
    public double TotalCarbs { get; set; }
    public double TotalProtein { get; set; }
    public double TotalFat { get; set; }

    public Guid UserId { get; set; }

    public DateTime DateTime { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime UpdateTime { get; set; }

    public IEnumerable<FoodItemDto> FoodItems { get; set; } = new List<FoodItemDto>();

}
