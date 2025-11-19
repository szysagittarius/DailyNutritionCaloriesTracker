namespace NutritionTracker.Application.UseCases.Nutrition;

public class FoodNutritionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Measurement { get; set; } = string.Empty;
    public double Carbs { get; set; }
    public double Fat { get; set; }
    public double Protein { get; set; }
    public double Calories { get; set; }
}
