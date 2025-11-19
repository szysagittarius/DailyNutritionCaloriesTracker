using NutritionTracker.Application.UseCases.FoodLogs;

namespace NutritionTracker.Api.Contracts.Responses;

/// <summary>
/// Response model for food log
/// Maps from Application layer DTOs
/// </summary>
public class FoodLogResponse
{
    public Guid Id { get; set; }
    public DateTime DateTime { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime UpdateTime { get; set; }
    public Guid UserId { get; set; }
    public double TotalCalories { get; set; }
    public double TotalCarbs { get; set; }
    public double TotalProtein { get; set; }
    public double TotalFat { get; set; }
    public List<FoodItemResponse> FoodItems { get; set; } = new();

    /// <summary>
    /// Create from Application DTO
    /// </summary>
    public static FoodLogResponse FromDto(FoodLogDto dto)
    {
        return new FoodLogResponse
        {
            Id = dto.Id,
            DateTime = dto.DateTime,
            CreateTime = dto.CreateTime,
            UpdateTime = dto.UpdateTime,
            UserId = dto.UserId,
            TotalCalories = dto.TotalCalories,
            TotalCarbs = dto.TotalCarbs,
            TotalProtein = dto.TotalProtein,
            TotalFat = dto.TotalFat,
            FoodItems = dto.FoodItems.Select(FoodItemResponse.FromDto).ToList()
        };
    }
}

public class FoodItemResponse
{
    public Guid Id { get; set; }
    public Guid FoodNutritionId { get; set; }
    public string FoodName { get; set; } = string.Empty;
    public string Measurement { get; set; } = string.Empty;
    public int Unit { get; set; }
    public double Calories { get; set; }
    public double Carbs { get; set; }
    public double Protein { get; set; }
    public double Fat { get; set; }

    public static FoodItemResponse FromDto(FoodLogItemDto dto)
    {
        return new FoodItemResponse
        {
            Id = dto.Id,
            FoodNutritionId = dto.FoodNutritionId,
            FoodName = dto.FoodName,
            Measurement = dto.Measurement,
            Unit = dto.Unit,
            Calories = dto.Calories,
            Carbs = dto.Carbs,
            Protein = dto.Protein,
            Fat = dto.Fat
        };
    }
}
