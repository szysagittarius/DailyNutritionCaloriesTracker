using NutritionTracker.Application.Ports.Output;
using NutritionTracker.Application.UseCases.FoodLogs.Commands;

namespace NutritionTracker.Application.UseCases.FoodLogs;

public class UpdateFoodLogUseCase
{
    private readonly IFoodLogRepository _foodLogRepository;
    private readonly IFoodNutritionRepository _foodNutritionRepository;

    public UpdateFoodLogUseCase(
        IFoodLogRepository foodLogRepository,
        IFoodNutritionRepository foodNutritionRepository)
    {
        _foodLogRepository = foodLogRepository;
        _foodNutritionRepository = foodNutritionRepository;
    }

    public async Task<FoodLogDto> ExecuteAsync(Guid foodLogId, DateTime dateTime, List<FoodItemDto> foodItems)
    {
        var foodLog = await _foodLogRepository.GetByIdAsync(foodLogId);
        
        if (foodLog == null)
            throw new InvalidOperationException($"FoodLog with ID {foodLogId} not found");

        // Clear existing food items
        foodLog.ClearFoodItems();

        // Add new food items
        foreach (var itemDto in foodItems)
        {
            var foodNutrition = await _foodNutritionRepository.GetByIdAsync(itemDto.FoodNutritionId);
            if (foodNutrition == null)
                throw new InvalidOperationException($"FoodNutrition with ID {itemDto.FoodNutritionId} not found");

            foodLog.AddFoodItem(itemDto.FoodNutritionId, itemDto.Unit, foodNutrition);
        }

        // Update date time if provided
        if (dateTime != default)
            foodLog.UpdateDateTime(dateTime);

        var updatedFoodLog = await _foodLogRepository.UpdateAsync(foodLog);

        return new FoodLogDto(
            updatedFoodLog.Id,
            updatedFoodLog.DateTime,
            updatedFoodLog.CreateTime,
            updatedFoodLog.UpdateTime,
            updatedFoodLog.UserId,
            updatedFoodLog.TotalCalories,
            updatedFoodLog.TotalCarbs,
            updatedFoodLog.TotalProtein,
            updatedFoodLog.TotalFat,
            updatedFoodLog.FoodItems.Select(fi => new FoodLogItemDto(
                fi.Id,
                fi.FoodNutritionId,
                fi.FoodNutrition?.Name ?? "",
                fi.FoodNutrition?.Measurement ?? "",
                fi.Unit,
                fi.CalculateCalories(),
                fi.CalculateCarbs(),
                fi.CalculateProtein(),
                fi.CalculateFat()
            )).ToList()
        );
    }
}
