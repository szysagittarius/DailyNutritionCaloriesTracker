namespace NutritionTracker.Api.Models
{
    public class UpdateUserProfileDto
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int SuggestedCalories { get; set; } = 2000;
        public int SuggestedCarbs { get; set; } = 0;
        public int SuggestedFat { get; set; } = 0;
        public int SuggestedProtein { get; set; } = 0;
    }
}