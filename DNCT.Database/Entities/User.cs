using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NT.Database.Entities;

[Table("Users")]
public record User
{
    [Key]
    [Column("Id", TypeName = "uniqueidentifier")]
    public required Guid Id { get; init; }

    [Column("Name", TypeName = "nvarchar(max)")]
    public required string Name { get; init; }

    [Column("Email", TypeName = "nvarchar(max)")]
    public required string Email { get; init; }

    [Column("Password", TypeName = "nvarchar(max)")]
    public required string Password { get; init; }

    [Column("SuggestedCalories", TypeName = "int")]
    public int SuggestedCalories { get; init; } = 2000;
}
