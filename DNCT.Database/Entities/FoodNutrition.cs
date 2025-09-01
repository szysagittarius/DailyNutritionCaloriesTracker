using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NT.Database.Entities;

[Table("FoodNutritions")]
public class FoodNutrition  // Changed from 'record class' to 'class'
{
    [Key]
    [Column("Id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }  // Changed from 'required' and 'init'

    [Column("Name")]
    public string Name { get; set; } = string.Empty;  // Changed from 'required' and 'init'

    [Column("Measurement")]
    public string Measurement { get; set; } = string.Empty;  // Changed from 'required' and 'init'

    [Column("Carbs")]
    public double Carbs { get; set; }  // Changed from 'required' and 'init'

    [Column("Fat")]
    public double Fat { get; set; }  // Changed from 'required' and 'init'

    [Column("Protein")]
    public double Protein { get; set; }  // Changed from 'required' and 'init'

    [Column("Calories")]
    public double Calories { get; set; }  // Changed from 'required' and 'init'

    // Navigation property
    public virtual ICollection<FoodItem> FoodItems { get; set; } = new List<FoodItem>();
}
