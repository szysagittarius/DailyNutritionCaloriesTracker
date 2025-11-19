using NutritionTracker.Application.UseCases.FoodLogs.Commands;

namespace NutritionTracker.Api.Contracts.Requests;

/// <summary>
/// Request model for creating a food log
/// </summary>
public class CreateFoodLogRequest
{
    public DateTime DateTime { get; set; }
    public Guid UserId { get; set; }
    public List<FoodItemRequest> FoodItems { get; set; } = new();

    /// <summary>
    /// Convert to Application Command
    /// </summary>
    public CreateFoodLogCommand ToCommand()
    {
        return new CreateFoodLogCommand(
            DateTime,
            UserId,
            FoodItems.Select(fi => new FoodItemDto(fi.FoodNutritionId, fi.Unit)).ToList()
        );
    }
}

public class FoodItemRequest
{
    public Guid FoodNutritionId { get; set; }
    public int Unit { get; set; }
}
