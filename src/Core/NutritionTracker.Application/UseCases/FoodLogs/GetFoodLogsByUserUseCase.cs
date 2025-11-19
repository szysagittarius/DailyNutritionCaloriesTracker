using NutritionTracker.Application.Ports.Output;
using NutritionTracker.Application.UseCases.FoodLogs.Queries;
using NutritionTracker.Domain.Entities;

namespace NutritionTracker.Application.UseCases.FoodLogs;

public interface IGetFoodLogsByUserUseCase
{
    Task<IEnumerable<FoodLogDto>> ExecuteAsync(GetFoodLogsByUserQuery query, CancellationToken cancellationToken = default);
}

public class GetFoodLogsByUserUseCase : IGetFoodLogsByUserUseCase
{
    private readonly IFoodLogRepository _foodLogRepository;

    public GetFoodLogsByUserUseCase(IFoodLogRepository foodLogRepository)
    {
        _foodLogRepository = foodLogRepository;
    }

    public async Task<IEnumerable<FoodLogDto>> ExecuteAsync(GetFoodLogsByUserQuery query, CancellationToken cancellationToken = default)
    {
        var foodLogs = await _foodLogRepository.GetByUserIdAsync(query.UserId, cancellationToken);
        return foodLogs.Select(MapToDto);
    }

    private FoodLogDto MapToDto(FoodLog foodLog)
    {
        return new FoodLogDto(
            foodLog.Id,
            foodLog.DateTime,
            foodLog.CreateTime,
            foodLog.UpdateTime,
            foodLog.UserId,
            foodLog.TotalCalories,
            foodLog.TotalCarbs,
            foodLog.TotalProtein,
            foodLog.TotalFat,
            foodLog.FoodItems.Select(fi => new FoodLogItemDto(
                fi.Id,
                fi.FoodNutritionId,
                fi.FoodNutrition.Name,
                fi.FoodNutrition.Measurement,
                fi.Unit,
                fi.CalculateCalories(),
                fi.CalculateCarbs(),
                fi.CalculateProtein(),
                fi.CalculateFat()
            )).ToList()
        );
    }
}
