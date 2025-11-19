using NT.Application.Contracts.Entities;

namespace NT.Application.Contracts.Ports;
public interface IFoodLogDataHandler
{
    Task<FoodLogEntity> GetAsync(Guid id);
    Task<IEnumerable<FoodLogEntity>> GetAllAsync();
    Task<IEnumerable<FoodLogEntity>> GetAllAsync(Guid userId);
    Task<FoodLogEntity> AddAsync(FoodLogEntity foodLog);
    Task<FoodLogEntity> UpdateAsync(FoodLogEntity foodLog);
    Task DeleteAsync(Guid id);
}
