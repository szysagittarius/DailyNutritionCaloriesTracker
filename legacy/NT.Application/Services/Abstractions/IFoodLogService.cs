using NT.Application.Contracts.Entities;

namespace NT.Application.Services.Abstractions;
public interface IFoodLogService
{
    //get, delete, update, add
    Task<FoodLogEntity> GetAsync(Guid id);
    Task<IEnumerable<FoodLogEntity>> GetAllAsync();
    Task<IEnumerable<FoodLogEntity>> GetAllAsync(Guid userId);
    Task<FoodLogEntity> AddAsync(FoodLogEntity foodLog);
    Task<FoodLogEntity> UpdateAsync(FoodLogEntity foodLog);
    Task DeleteAsync(Guid id);
    Task DeleteAsync(IEnumerable<Guid> ids);
    Task DeleteAsync();

}
