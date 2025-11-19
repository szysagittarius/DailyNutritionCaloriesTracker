using NT.Application.Contracts.Entities;

namespace NT.Application.Contracts.Ports;

public interface IFoodNutritionDataHandler
{
    Task<FoodNutritionEntity> GetAsync(string foodName);
    Task<IEnumerable<FoodNutritionEntity>> GetAsync(IEnumerable<string> foodNames);
    Task<IEnumerable<FoodNutritionEntity>> GetAllAsync();
    Task<FoodNutritionEntity> AddAsync(FoodNutritionEntity FoodNutrition);
    Task<FoodNutritionEntity> UpdateAsync(FoodNutritionEntity FoodNutrition);
    Task DeleteAsync(string foodName);
    Task DeleteAsync(IEnumerable<string> foodNames);
    Task DeleteAsync();
}
