using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NutritionTracker.SqlServer.Entities;

[Table("FoodNutritions")]
public class FoodNutritionEntity
{
    [Key]
    [Column("Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [Column("Name")]
    public string Name { get; set; } = string.Empty;

    [Column("Measurement")]
    public string Measurement { get; set; } = string.Empty;

    [Column("Carbs")]
    public double Carbs { get; set; }

    [Column("Fat")]
    public double Fat { get; set; }

    [Column("Protein")]
    public double Protein { get; set; }

    [Column("Calories")]
    public double Calories { get; set; }

    public virtual ICollection<FoodItemEntity> FoodItems { get; set; } = new List<FoodItemEntity>();
}
