using NutritionTracker.Domain.Entities;

namespace NutritionTracker.Application.Ports.Output;

/// <summary>
/// Output port for FoodNutrition persistence operations
/// </summary>
public interface IFoodNutritionRepository
{
    Task<FoodNutrition?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<FoodNutrition>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<FoodNutrition>> SearchByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<FoodNutrition> AddAsync(FoodNutrition foodNutrition, CancellationToken cancellationToken = default);
    Task<FoodNutrition> UpdateAsync(FoodNutrition foodNutrition, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
