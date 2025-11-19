using Microsoft.EntityFrameworkCore;
using NT.Database.Context;
using NT.Database.Entities;
using NT.Ef.Repositories.Abstractions;

namespace NT.Ef.Repositories.Implementations.Repositories;
internal class FoodLogRepository : BaseRepository<FoodLog>, IFoodLogRepository
{
    public FoodLogRepository(NutritionTrackerDbContext dbContext) : base(dbContext)
    {
    }

    // Override the GetAll method to include FoodItems AND their FoodNutrition
    public new IQueryable<FoodLog> GetAll()
    {
        return dbContext.Set<FoodLog>()
            .Include(fl => fl.FoodItems)
                .ThenInclude(fi => fi.FoodNutrition); // Include the FoodNutrition for each FoodItem
    }

    // Override GetByIdAsync to include FoodItems and their FoodNutrition
    public new async Task<FoodLog> GetByIdAsync(Guid id)
    {
        return await dbContext.Set<FoodLog>()
            .Include(fl => fl.FoodItems)
                .ThenInclude(fi => fi.FoodNutrition)
            .FirstOrDefaultAsync(fl => fl.Id == id);
    }
}
