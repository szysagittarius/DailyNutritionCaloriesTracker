using NutritionTracker.Application.Ports.Output;
using NutritionTracker.Domain.Entities;

namespace NutritionTracker.Application.UseCases.Users;

public class CreateUserUseCase
{
    private readonly IUserRepository _userRepository;

    public CreateUserUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserDto> ExecuteAsync(string name, string email, string password, 
        double suggestedCalories, double suggestedCarbs, double suggestedFat, double suggestedProtein)
    {
        var user = User.Create(name, email, password, suggestedCalories, 
            suggestedCarbs, suggestedFat, suggestedProtein);
        
        var savedUser = await _userRepository.AddAsync(user);

        return new UserDto
        {
            Id = savedUser.Id,
            Name = savedUser.Name,
            Email = savedUser.Email,
            SuggestedCalories = savedUser.SuggestedCalories,
            SuggestedCarbs = savedUser.SuggestedCarbs,
            SuggestedFat = savedUser.SuggestedFat,
            SuggestedProtein = savedUser.SuggestedProtein
        };
    }
}
