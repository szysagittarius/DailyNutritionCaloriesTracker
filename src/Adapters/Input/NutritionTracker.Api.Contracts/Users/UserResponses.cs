namespace NutritionTracker.Api.Contracts.Users;

public class UserResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Guid? RoleId { get; set; }
    public string RoleName { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public double SuggestedCalories { get; set; }
    public double SuggestedCarbs { get; set; }
    public double SuggestedFat { get; set; }
    public double SuggestedProtein { get; set; }
}

public class UserRoleResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class LoginResponse
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public Guid RoleId { get; set; }
    public bool IsAdmin { get; set; }
}
