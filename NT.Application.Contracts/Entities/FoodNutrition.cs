namespace NT.Application.Contracts.Entities;

public class FoodNutrition
{
    public required string Name { get; set; }
    public required string Measurement { get; set; }
    public double Calories { get; set; }
    public double Protein { get; set; }
    public double Carbs { get; set; }
    public double Fat { get; set; }
}
