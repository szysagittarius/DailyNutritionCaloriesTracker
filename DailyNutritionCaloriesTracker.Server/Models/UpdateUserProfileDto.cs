namespace NutritionTracker.Api.Models
{
    public class UpdateUserProfileDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Password { get; set; }
        public int SuggestedCalories { get; set; } = 2000;
    }
}