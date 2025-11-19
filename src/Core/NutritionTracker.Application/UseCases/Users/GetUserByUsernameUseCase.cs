using NutritionTracker.Application.Ports.Output;

namespace NutritionTracker.Application.UseCases.Users;

public class GetUserByUsernameUseCase
{
    private readonly IUserRepository _userRepository;

    public GetUserByUsernameUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto?> ExecuteAsync(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        
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
