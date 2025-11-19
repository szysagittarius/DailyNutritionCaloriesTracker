using NutritionTracker.Application.Ports.Output;

namespace NutritionTracker.Application.UseCases.Nutrition;

public class GetAllFoodNutritionUseCase
{
    private readonly IFoodNutritionRepository _foodNutritionRepository;

    public GetAllFoodNutritionUseCase(IFoodNutritionRepository foodNutritionRepository)
    {
        _foodNutritionRepository = foodNutritionRepository;
    }

    public async Task<IEnumerable<FoodNutritionDto>> ExecuteAsync()
    {
        var foodNutritions = await _foodNutritionRepository.GetAllAsync();
        
        return foodNutritions.Select(fn => new FoodNutritionDto
        {
            Id = fn.Id,
            Name = fn.Name,
            Measurement = fn.Measurement,
            Carbs = fn.Carbs,
            Fat = fn.Fat,
            Protein = fn.Protein,
            Calories = fn.Calories
        });
    }
}
