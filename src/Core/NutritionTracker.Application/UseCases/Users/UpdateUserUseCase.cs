using NutritionTracker.Application.Ports.Output;

namespace NutritionTracker.Application.UseCases.Users;

public class UpdateUserUseCase
{
    private readonly IUserRepository _userRepository;

    public UpdateUserUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> ExecuteAsync(Guid userId, string? name, string? email, 
        string? password, double? suggestedCalories, double? suggestedCarbs, 
        double? suggestedFat, double? suggestedProtein)
    {
        var user = await _userRepository.GetByIdAsync(userId) 
            ?? throw new InvalidOperationException($"User with ID {userId} not found");

        if (!string.IsNullOrEmpty(name)) user.UpdateName(name);
        if (!string.IsNullOrEmpty(email)) user.UpdateEmail(email);
        if (!string.IsNullOrEmpty(password)) user.UpdatePassword(password);
        
        if (suggestedCalories.HasValue || suggestedCarbs.HasValue || 
            suggestedFat.HasValue || suggestedProtein.HasValue)
        {
            user.UpdateNutritionalGoals(
                suggestedCalories ?? user.SuggestedCalories,
                suggestedCarbs ?? user.SuggestedCarbs,
                suggestedFat ?? user.SuggestedFat,
                suggestedProtein ?? user.SuggestedProtein
            );
        }

        var updatedUser = await _userRepository.UpdateAsync(user);

        return new UserDto
        {
            Id = updatedUser.Id,
            Name = updatedUser.Name,
            Email = updatedUser.Email,
            SuggestedCalories = updatedUser.SuggestedCalories,
            SuggestedCarbs = updatedUser.SuggestedCarbs,
            SuggestedFat = updatedUser.SuggestedFat,
            SuggestedProtein = updatedUser.SuggestedProtein
        };
    }
}
