namespace NutritionTracker.Application.UseCases.FoodLogs;

public record FoodLogDto(
    Guid Id,
    DateTime DateTime,
    DateTime CreateTime,
    DateTime UpdateTime,
    Guid UserId,
    double TotalCalories,
    double TotalCarbs,
    double TotalProtein,
    double TotalFat,
    List<FoodLogItemDto> FoodItems
);

public record FoodLogItemDto(
    Guid Id,
    Guid FoodNutritionId,
    string FoodName,
    string Measurement,
    int Unit,
    double Calories,
    double Carbs,
    double Protein,
    double Fat
);
