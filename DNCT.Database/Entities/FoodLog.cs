using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NT.Database.Entities;

[Table("FoodLogs")]
public class FoodLog  // Changed from 'record' to 'class'
{
    [Key]
    [Column("Id", TypeName = "uniqueidentifier")]
    public Guid Id { get; set; }  // Changed from 'required' and 'init'

    [Column("DateTime", TypeName = "datetime")]
    public DateTime DateTime { get; set; }  // Changed from 'required' and 'init'

    [Column("CreateTime", TypeName = "datetime")]
    public DateTime CreateTime { get; set; }  // Changed from 'required' and 'init'

    [Column("UpdateTime", TypeName = "datetime")]
    public DateTime UpdateTime { get; set; }  // Changed from 'required' and 'init'

    [ForeignKey("User")]
    [Column("UserId", TypeName = "uniqueidentifier")]
    public Guid UserId { get; set; }  // Changed from 'required' and 'init'

    // Navigation properties
    public virtual User? User { get; set; }  // Changed from 'required' and 'init', made nullable

    public virtual ICollection<FoodItem> FoodItems { get; set; } = new List<FoodItem>();  // Changed from 'init'

    // Add calculated properties for totals
    [Column("TotalCalories", TypeName = "float")]
    public double TotalCalories { get; set; }

    [Column("TotalCarbs", TypeName = "float")]
    public double TotalCarbs { get; set; }

    [Column("TotalProtein", TypeName = "float")]
    public double TotalProtein { get; set; }

    [Column("TotalFat", TypeName = "float")]
    public double TotalFat { get; set; }
}
