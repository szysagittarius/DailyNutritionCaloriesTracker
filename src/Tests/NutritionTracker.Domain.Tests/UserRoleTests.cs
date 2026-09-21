using NutritionTracker.Domain.Entities;

namespace NutritionTracker.Domain.Tests;

public class UserRoleTests
{
    [Fact]
    public void Default_new_user_should_get_user_role()
    {
        var user = User.Create("alice", "alice@example.com", "Password123!");

        Assert.Equal(UserRole.DefaultUserRoleId, user.RoleId);
        Assert.Equal(UserRole.UserRoleName, user.RoleName);
        Assert.False(user.IsAdmin);
    }

    [Fact]
    public void Admin_user_should_be_flagged()
    {
        var user = new User(
            Guid.NewGuid(),
            "admin",
            "admin@example.com",
            "Password123!",
            roleId: UserRole.DefaultAdminRoleId);

        Assert.Equal(UserRole.DefaultAdminRoleId, user.RoleId);
        Assert.Equal(UserRole.AdminRoleName, user.RoleName);
        Assert.True(user.IsAdmin);
    }
}
