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

        // Allow signing in with the email address as well as the display name
        if (user == null && username.Contains('@'))
            user = await _userRepository.GetByEmailAsync(username);

        if (user == null)
            return null;

        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Password = user.Password,
            RoleId = user.RoleId,
            RoleName = user.RoleName,
            IsAdmin = user.IsAdmin,
            SuggestedCalories = user.SuggestedCalories,
            SuggestedCarbs = user.SuggestedCarbs,
            SuggestedFat = user.SuggestedFat,
            SuggestedProtein = user.SuggestedProtein
        };
    }
}
