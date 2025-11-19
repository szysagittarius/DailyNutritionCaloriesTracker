using NT.Application.Contracts.Entities;

namespace NT.Application.Services.Abstractions;
public interface IFoodNutritionService
{
    //Task<FoodNutritionEntity> GetFoodNutritionAsync(string foodName);
    Task<IEnumerable<FoodNutritionEntity>> GetFoodNutritionAsync(IEnumerable<string> foodNames);
    Task<IEnumerable<FoodNutritionEntity>> GetFoodNutritionAsync();
    Task<FoodNutritionEntity> AddFoodNutritionAsync(FoodNutritionEntity FoodNutrition);
    Task<FoodNutritionEntity> UpdateFoodNutritionAsync(FoodNutritionEntity FoodNutrition);
    Task DeleteFoodNutritionAsync(string foodName);
    Task DeleteFoodNutritionAsync(IEnumerable<string> foodNames);
    Task DeleteFoodNutritionAsync();
}
