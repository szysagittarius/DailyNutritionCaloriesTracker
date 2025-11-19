using NutritionTracker.Application.Ports.Output;

namespace NutritionTracker.Application.UseCases.FoodLogs;

public class DeleteFoodLogUseCase
{
    private readonly IFoodLogRepository _foodLogRepository;

    public DeleteFoodLogUseCase(IFoodLogRepository foodLogRepository)
    {
        _foodLogRepository = foodLogRepository;
    }

    public async Task ExecuteAsync(Guid foodLogId)
    {
        var foodLog = await _foodLogRepository.GetByIdAsync(foodLogId);
        
        if (foodLog == null)
            throw new InvalidOperationException($"FoodLog with ID {foodLogId} not found");

        await _foodLogRepository.DeleteAsync(foodLog);
    }
}
