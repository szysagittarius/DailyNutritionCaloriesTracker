using NutritionTracker.Application.Ports.Output;

namespace NutritionTracker.Application.UseCases.UserRoles;

public class GetAllUserRolesUseCase
{
    private readonly IUserRoleRepository _userRoleRepository;

    public GetAllUserRolesUseCase(IUserRoleRepository userRoleRepository)
    {
        _userRoleRepository = userRoleRepository;
    }

    public async Task<IEnumerable<UserRoleDto>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        await _userRoleRepository.EnsureDefaultRolesAsync(cancellationToken);

        var roles = await _userRoleRepository.GetAllAsync(cancellationToken);

        return roles.Select(role => new UserRoleDto
        {
            Id = role.Id,
            Name = role.Name
        });
    }
}
