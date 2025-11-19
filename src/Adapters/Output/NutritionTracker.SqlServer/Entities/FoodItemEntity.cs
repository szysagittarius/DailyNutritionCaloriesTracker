using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NutritionTracker.SqlServer.Entities;

[Table("FoodItems")]
public class FoodItemEntity
{
    [Key]
    [Column("Id", TypeName = "uniqueidentifier")]
    public Guid Id { get; set; }

    [ForeignKey("FoodNutrition")]
    [Column("FoodNutritionId", TypeName = "uniqueidentifier")]
    public Guid FoodNutritionId { get; set; }

    public virtual FoodNutritionEntity? FoodNutrition { get; set; }

    [Column("Unit", TypeName = "int")]
    public int Unit { get; set; }

    [ForeignKey("FoodLog")]
    [Column("FoodLogId", TypeName = "uniqueidentifier")]
    public Guid FoodLogId { get; set; }

    public virtual FoodLogEntity? FoodLog { get; set; }
}
