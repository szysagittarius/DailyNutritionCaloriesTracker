namespace NutritionTracker.Domain.Entities;

public class FoodItem
{
    public Guid Id { get; private set; }
    public Guid FoodNutritionId { get; private set; }
    public FoodNutrition FoodNutrition { get; private set; }
    public int Unit { get; private set; }
    public Guid FoodLogId { get; private set; }

    private FoodItem() { } // For EF Core

    public FoodItem(Guid id, Guid foodNutritionId, int unit, Guid foodLogId, FoodNutrition foodNutrition)
    {
        if (foodNutritionId == Guid.Empty)
            throw new ArgumentException("FoodNutritionId cannot be empty", nameof(foodNutritionId));
        if (foodLogId == Guid.Empty)
            throw new ArgumentException("FoodLogId cannot be empty", nameof(foodLogId));
        if (unit <= 0)
            throw new ArgumentException("Unit must be positive", nameof(unit));
        if (foodNutrition == null)
            throw new ArgumentNullException(nameof(foodNutrition));

        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        FoodNutritionId = foodNutritionId;
        Unit = unit;
        FoodLogId = foodLogId;
        FoodNutrition = foodNutrition;
    }

    public static FoodItem Create(Guid foodNutritionId, int unit, Guid foodLogId, FoodNutrition foodNutrition)
    {
        return new FoodItem(Guid.NewGuid(), foodNutritionId, unit, foodLogId, foodNutrition);
    }

    public double CalculateCalories() => FoodNutrition.Calories * Unit;
    public double CalculateCarbs() => FoodNutrition.Carbs * Unit;
    public double CalculateProtein() => FoodNutrition.Protein * Unit;
    public double CalculateFat() => FoodNutrition.Fat * Unit;

    public void UpdateUnit(int unit)
    {
        if (unit <= 0)
            throw new ArgumentException("Unit must be positive", nameof(unit));
        Unit = unit;
    }
}
