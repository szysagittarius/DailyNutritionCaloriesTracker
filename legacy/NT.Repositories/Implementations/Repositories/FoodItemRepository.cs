
using NT.Database.Context;
using NT.Database.Entities;
using NT.Ef.Repositories.Abstractions;

namespace NT.Ef.Repositories.Implementations.Repositories;
internal class FoodItemRepository : BaseRepository<FoodItem>, IFoodItemRepository
{
    public FoodItemRepository(NutritionTrackerDbContext dbContext) : base(dbContext)
    {
    }
}
