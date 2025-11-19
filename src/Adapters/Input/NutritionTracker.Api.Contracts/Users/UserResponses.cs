namespace NutritionTracker.Api.Contracts.Users;

public class UserResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public double SuggestedCalories { get; set; }
    public double SuggestedCarbs { get; set; }
    public double SuggestedFat { get; set; }
    public double SuggestedProtein { get; set; }
}
