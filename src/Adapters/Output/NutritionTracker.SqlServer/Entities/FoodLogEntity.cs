using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NutritionTracker.SqlServer.Entities;

[Table("FoodLogs")]
public class FoodLogEntity
{
    [Key]
    [Column("Id", TypeName = "uniqueidentifier")]
    public Guid Id { get; set; }

    [Column("DateTime", TypeName = "datetime")]
    public DateTime DateTime { get; set; }

    [Column("CreateTime", TypeName = "datetime")]
    public DateTime CreateTime { get; set; }

    [Column("UpdateTime", TypeName = "datetime")]
    public DateTime UpdateTime { get; set; }

    [ForeignKey("User")]
    [Column("UserId", TypeName = "uniqueidentifier")]
    public Guid UserId { get; set; }

    public virtual UserEntity? User { get; set; }

    public virtual ICollection<FoodItemEntity> FoodItems { get; set; } = new List<FoodItemEntity>();

    [Column("TotalCalories", TypeName = "float")]
    public double TotalCalories { get; set; }

    [Column("TotalCarbs", TypeName = "float")]
    public double TotalCarbs { get; set; }

    [Column("TotalProtein", TypeName = "float")]
    public double TotalProtein { get; set; }

    [Column("TotalFat", TypeName = "float")]
    public double TotalFat { get; set; }
}
