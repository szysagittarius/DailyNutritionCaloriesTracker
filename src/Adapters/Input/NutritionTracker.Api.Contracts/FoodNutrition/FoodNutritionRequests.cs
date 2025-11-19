using System.ComponentModel.DataAnnotations;

namespace NutritionTracker.Api.Contracts.FoodNutrition;

public class CreateFoodNutritionRequest
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Measurement is required")]
    public string Measurement { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "Carbs must be positive")]
    public double Carbs { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Fat must be positive")]
    public double Fat { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Protein must be positive")]
    public double Protein { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Calories must be positive")]
    public double Calories { get; set; }
}
