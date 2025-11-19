using NutritionTracker.Application.Ports.Output;
using NutritionTracker.Application.UseCases.FoodLogs.Commands;
using NutritionTracker.Domain.Entities;

namespace NutritionTracker.Application.UseCases.FoodLogs;

public interface ICreateFoodLogUseCase
{
    Task<FoodLogDto> ExecuteAsync(CreateFoodLogCommand command, CancellationToken cancellationToken = default);
}

public class CreateFoodLogUseCase : ICreateFoodLogUseCase
{
    private readonly IFoodLogRepository _foodLogRepository;
    private readonly IFoodNutritionRepository _foodNutritionRepository;
    private readonly IUserRepository _userRepository;

    public CreateFoodLogUseCase(
        IFoodLogRepository foodLogRepository,
        IFoodNutritionRepository foodNutritionRepository,
        IUserRepository userRepository)
    {
        _foodLogRepository = foodLogRepository;
        _foodNutritionRepository = foodNutritionRepository;
        _userRepository = userRepository;
    }

    public async Task<FoodLogDto> ExecuteAsync(CreateFoodLogCommand command, CancellationToken cancellationToken = default)
    {
        // Validate user exists
        var user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user == null)
            throw new InvalidOperationException($"User with ID {command.UserId} not found");

        // Create food log
        var foodLog = new FoodLog(Guid.NewGuid(), command.DateTime, command.UserId);

        // Add food items
        foreach (var item in command.FoodItems)
        {
            var foodNutrition = await _foodNutritionRepository.GetByIdAsync(item.FoodNutritionId, cancellationToken);
            if (foodNutrition == null)
                throw new InvalidOperationException($"FoodNutrition with ID {item.FoodNutritionId} not found");

            var foodItem = new FoodItem(Guid.NewGuid(), item.FoodNutritionId, item.Unit, foodLog.Id, foodNutrition);
            foodLog.AddFoodItem(foodItem);
        }

        // Save
        var savedFoodLog = await _foodLogRepository.AddAsync(foodLog, cancellationToken);

        // Map to DTO
        return MapToDto(savedFoodLog);
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
