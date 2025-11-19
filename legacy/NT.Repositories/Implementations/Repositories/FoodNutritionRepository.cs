
using NT.Database.Context;
using NT.Database.Entities;
using NT.Ef.Repositories.Abstractions;

namespace NT.Ef.Repositories.Implementations.Repositories;
public class FoodNutritionRepository : BaseRepository<FoodNutrition>, IFoodNutritionRepository
{
    public FoodNutritionRepository(NutritionTrackerDbContext dbContext) : base(dbContext)
    {

    }
}
