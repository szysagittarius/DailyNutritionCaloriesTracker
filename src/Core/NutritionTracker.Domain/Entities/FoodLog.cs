namespace NutritionTracker.Domain.Entities;

public class FoodLog
{
    public Guid Id { get; private set; }
    public DateTime DateTime { get; private set; }
    public DateTime CreateTime { get; private set; }
    public DateTime UpdateTime { get; private set; }
    public Guid UserId { get; private set; }
    
    private readonly List<FoodItem> _foodItems = new();
    public IReadOnlyCollection<FoodItem> FoodItems => _foodItems.AsReadOnly();

    public double TotalCalories { get; private set; }
    public double TotalCarbs { get; private set; }
    public double TotalProtein { get; private set; }
    public double TotalFat { get; private set; }

    private FoodLog() { } // For EF Core

    public FoodLog(Guid id, DateTime dateTime, Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("UserId cannot be empty", nameof(userId));

        Id = id == Guid.Empty ? Guid.NewGuid() : id;
        DateTime = dateTime;
        UserId = userId;
        CreateTime = System.DateTime.UtcNow;
        UpdateTime = System.DateTime.UtcNow;
    }

    public void AddFoodItem(FoodItem foodItem)
    {
        _foodItems.Add(foodItem);
        RecalculateTotals();
        UpdateTime = System.DateTime.UtcNow;
    }

    public void RemoveFoodItem(FoodItem foodItem)
    {
        _foodItems.Remove(foodItem);
        RecalculateTotals();
        UpdateTime = System.DateTime.UtcNow;
    }

    private void RecalculateTotals()
    {
        TotalCalories = _foodItems.Sum(fi => fi.CalculateCalories());
        TotalCarbs = _foodItems.Sum(fi => fi.CalculateCarbs());
        TotalProtein = _foodItems.Sum(fi => fi.CalculateProtein());
        TotalFat = _foodItems.Sum(fi => fi.CalculateFat());
    }

    public void UpdateDateTime(DateTime dateTime)
    {
        DateTime = dateTime;
        UpdateTime = System.DateTime.UtcNow;
    }

    public void ClearFoodItems()
    {
        _foodItems.Clear();
        RecalculateTotals();
        UpdateTime = System.DateTime.UtcNow;
    }

    public void AddFoodItem(Guid foodNutritionId, int unit, FoodNutrition foodNutrition)
    {
        var foodItem = FoodItem.Create(foodNutritionId, unit, Id, foodNutrition);
        _foodItems.Add(foodItem);
        RecalculateTotals();
        UpdateTime = System.DateTime.UtcNow;
    }
}
