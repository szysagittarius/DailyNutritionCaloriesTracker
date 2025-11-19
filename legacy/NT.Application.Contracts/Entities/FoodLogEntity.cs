namespace NT.Application.Contracts.Entities;
public class FoodLogEntity
{
    public Guid Id { get; set; } // Required for Entity Framework
    public double TotalCalories { get; set; }
    public double TotalCarbs { get; set; }
    public double TotalProtein { get; set; }
    public double TotalFat { get; set; }

    public Guid UserId { get; set; }

    public DateTime DateTime { get; set; }
    public DateTime CreateTime { get; set; }
    public DateTime UpdateTime { get; set; }
    
    // CHANGE: Use ICollection instead of IEnumerable for Entity Framework navigation properties
    public ICollection<FoodItemEntity> FoodItems { get; set; } = new List<FoodItemEntity>();
}
