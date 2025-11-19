namespace NutritionTracker.Application.UseCases.FoodLogs.Commands;

public record CreateFoodLogCommand(
    DateTime DateTime,
    Guid UserId,
    List<FoodItemDto> FoodItems
);

public record FoodItemDto(
    Guid FoodNutritionId,
    int Unit
);
