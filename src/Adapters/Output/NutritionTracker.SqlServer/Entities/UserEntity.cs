using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NutritionTracker.SqlServer.Entities;

[Table("Users")]
public class UserEntity
{
    [Key]
    [Column("Id", TypeName = "uniqueidentifier")]
    public Guid Id { get; set; }

    [Column("Name", TypeName = "nvarchar(max)")]
    public string Name { get; set; } = string.Empty;

    [Column("Email", TypeName = "nvarchar(max)")]
    public string Email { get; set; } = string.Empty;

    [Column("Password", TypeName = "nvarchar(max)")]
    public string Password { get; set; } = string.Empty;

    [Column("SuggestedCalories")]
    public double SuggestedCalories { get; set; } = 2000;

    [Column("SuggestedCarbs")]
    public double SuggestedCarbs { get; set; } = 246;

    [Column("SuggestedFat")]
    public double SuggestedFat { get; set; } = 68;

    [Column("SuggestedProtein")]
    public double SuggestedProtein { get; set; } = 215;

    public virtual ICollection<FoodLogEntity> FoodLogs { get; set; } = new List<FoodLogEntity>();
}
