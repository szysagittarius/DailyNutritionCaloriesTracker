namespace NutritionTracker.Domain.Entities;

public class UserRole
{
    public const string UserRoleName = "User";
    public const string AdminRoleName = "Admin";

    public static readonly Guid DefaultUserRoleId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public static readonly Guid DefaultAdminRoleId = Guid.Parse("22222222-2222-2222-2222-222222222222");

    public Guid Id { get; private set; }
    public string Name { get; private set; }

    private UserRole() { }

    public UserRole(Guid id, string name)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Role id cannot be empty", nameof(id));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name cannot be empty", nameof(name));

        Id = id;
        Name = name;
    }

    public static UserRole User => new(DefaultUserRoleId, UserRoleName);
    public static UserRole Admin => new(DefaultAdminRoleId, AdminRoleName);
}
