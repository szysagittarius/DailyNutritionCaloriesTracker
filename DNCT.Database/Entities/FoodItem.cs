using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NT.Database.Entities;

[Table("FoodItems")]
public class FoodItem  // Changed from 'record' to 'class'
{
    [Key]
    [Column("Id", TypeName = "uniqueidentifier")]
    public Guid Id { get; set; }  // Changed from 'required' and 'init' to regular setter

    [ForeignKey("FoodNutrition")]
    [Column("FoodNutritionId", TypeName = "uniqueidentifier")]
    public Guid FoodNutritionId { get; set; }  // Changed from 'required' and 'init'

    public virtual FoodNutrition? FoodNutrition { get; set; }  // Removed 'required', added 'virtual' and nullable

    [Column("Unit", TypeName = "int")]
    public int Unit { get; set; }  // Changed from 'required' and 'init'

    [ForeignKey("FoodLog")]
    [Column("FoodLogId", TypeName = "uniqueidentifier")]
    public Guid FoodLogId { get; set; }  // Changed from 'required' and 'init'

    public virtual FoodLog? FoodLog { get; set; }  // Removed 'required', added 'virtual' and nullable
}
