using NutritionTracker.Domain.Entities;

namespace NutritionTracker.Application.Ports.Output;

public interface IUserRoleRepository
{
    Task<IEnumerable<UserRole>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<UserRole?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<UserRole?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<UserRole> AddAsync(UserRole role, CancellationToken cancellationToken = default);
    Task EnsureDefaultRolesAsync(CancellationToken cancellationToken = default);
}
