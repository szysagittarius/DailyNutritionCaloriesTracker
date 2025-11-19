using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NT.Database.Entities;

[Table("Users")]
public class User  // Changed from 'record' to 'class'
{
    [Key]
    [Column("Id", TypeName = "uniqueidentifier")]
    public Guid Id { get; set; }  // Changed from 'required' and 'init' to regular setter

    [Column("Name", TypeName = "nvarchar(max)")]
    public string Name { get; set; } = string.Empty;  // Changed from 'required' and 'init'

    [Column("Email", TypeName = "nvarchar(max)")]
    public string Email { get; set; } = string.Empty;  // Changed from 'required' and 'init'

    [Column("Password", TypeName = "nvarchar(max)")]
    public string Password { get; set; } = string.Empty;  // Changed from 'required' and 'init'

    [Column("SuggestedCalories", TypeName = "int")]
    public int SuggestedCalories { get; set; } = 2000;  // Changed from 'init'

    [Column("SuggestedCarbs", TypeName = "int")]
    public int SuggestedCarbs { get; set; } = 246;  // ADD THIS

    [Column("SuggestedFat", TypeName = "int")]
    public int SuggestedFat { get; set; } = 68;  // ADD THIS

    [Column("SuggestedProtein", TypeName = "int")]
    public int SuggestedProtein { get; set; } = 215;  // ADD THIS

    // Navigation property
    public virtual ICollection<FoodLog> FoodLogs { get; set; } = new List<FoodLog>();
}
