using NutritionTracker.Application.Ports.Output;

namespace NutritionTracker.Application.UseCases.FoodLogs;

public class GetAllFoodLogsUseCase
{
    private readonly IFoodLogRepository _foodLogRepository;

    public GetAllFoodLogsUseCase(IFoodLogRepository foodLogRepository)
    {
        _foodLogRepository = foodLogRepository;
    }

    public async Task<IEnumerable<FoodLogDto>> ExecuteAsync()
    {
        var foodLogs = await _foodLogRepository.GetAllAsync();
        
        return foodLogs.Select(fl => new FoodLogDto(
            fl.Id,
            fl.DateTime,
            fl.CreateTime,
            fl.UpdateTime,
            fl.UserId,
            fl.TotalCalories,
            fl.TotalCarbs,
            fl.TotalProtein,
            fl.TotalFat,
            fl.FoodItems.Select(fi => new FoodLogItemDto(
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
        ));
    }
}
