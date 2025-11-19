using NutritionTracker.Domain.Entities;

namespace NutritionTracker.Application.Ports.Output;

/// <summary>
/// Output port for FoodLog persistence operations
/// </summary>
public interface IFoodLogRepository
{
    Task<FoodLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<FoodLog>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<FoodLog>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<FoodLog> AddAsync(FoodLog foodLog, CancellationToken cancellationToken = default);
    Task<FoodLog> UpdateAsync(FoodLog foodLog, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task DeleteAsync(FoodLog foodLog, CancellationToken cancellationToken = default);
    Task DeleteByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task DeleteAllAsync(CancellationToken cancellationToken = default);
}
