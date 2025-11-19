namespace NutritionTracker.Api.Models;

public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public int SuggestedCalories { get; set; }
    public int SuggestedCarbs { get; set; }      // ADD THIS
    public int SuggestedFat { get; set; }        // ADD THIS
    public int SuggestedProtein { get; set; }    // ADD THIS
}
