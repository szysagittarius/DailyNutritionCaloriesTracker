using NutritionTracker.Application.Ports.Output;

namespace NutritionTracker.Application.UseCases.Users;

public class GetAllUsersUseCase
{
    private readonly IUserRepository _userRepository;

    public GetAllUsersUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<UserDto>> ExecuteAsync()
    {
        var users = await _userRepository.GetAllAsync();
        
        return users.Select(user => new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            SuggestedCalories = user.SuggestedCalories,
            SuggestedCarbs = user.SuggestedCarbs,
            SuggestedFat = user.SuggestedFat,
            SuggestedProtein = user.SuggestedProtein
        });
    }
}
