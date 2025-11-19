using NutritionTracker.Application.Ports.Output;

namespace NutritionTracker.Application.UseCases.Users;

public class GetUserByIdUseCase
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto?> ExecuteAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        
        if (user == null)
            return null;

        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            SuggestedCalories = user.SuggestedCalories,
            SuggestedCarbs = user.SuggestedCarbs,
            SuggestedFat = user.SuggestedFat,
            SuggestedProtein = user.SuggestedProtein
        };
    }
}
