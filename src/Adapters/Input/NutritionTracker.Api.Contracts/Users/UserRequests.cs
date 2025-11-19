using System.ComponentModel.DataAnnotations;

namespace NutritionTracker.Api.Contracts.Users;

public class CreateUserRequest
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string Password { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "Suggested calories must be positive")]
    public double SuggestedCalories { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Suggested carbs must be positive")]
    public double SuggestedCarbs { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Suggested fat must be positive")]
    public double SuggestedFat { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Suggested protein must be positive")]
    public double SuggestedProtein { get; set; }
}

public class UpdateUserRequest
{
    public string? Name { get; set; }
    
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string? Email { get; set; }
    
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters")]
    public string? Password { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Suggested calories must be positive")]
    public double? SuggestedCalories { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Suggested carbs must be positive")]
    public double? SuggestedCarbs { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Suggested fat must be positive")]
    public double? SuggestedFat { get; set; }
    
    [Range(0, double.MaxValue, ErrorMessage = "Suggested protein must be positive")]
    public double? SuggestedProtein { get; set; }
}
