using NutritionTracker.Application.Ports.Output;
using NutritionTracker.Domain.Entities;

namespace NutritionTracker.Application.UseCases.Nutrition;

public class CreateFoodNutritionUseCase
{
    private readonly IFoodNutritionRepository _foodNutritionRepository;

    public CreateFoodNutritionUseCase(IFoodNutritionRepository foodNutritionRepository)
    {
        _foodNutritionRepository = foodNutritionRepository;
    }

    public async Task<FoodNutritionDto> ExecuteAsync(string name, string measurement, 
        double carbs, double fat, double protein, double calories)
    {
        var foodNutrition = FoodNutrition.Create(name, measurement, carbs, fat, protein, calories);
        
        var savedFoodNutrition = await _foodNutritionRepository.AddAsync(foodNutrition);

        return new FoodNutritionDto
        {
            Id = savedFoodNutrition.Id,
            Name = savedFoodNutrition.Name,
            Measurement = savedFoodNutrition.Measurement,
            Carbs = savedFoodNutrition.Carbs,
            Fat = savedFoodNutrition.Fat,
            Protein = savedFoodNutrition.Protein,
            Calories = savedFoodNutrition.Calories
        };
    }
}
