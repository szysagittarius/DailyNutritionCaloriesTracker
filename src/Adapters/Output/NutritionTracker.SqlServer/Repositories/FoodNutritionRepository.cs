using Microsoft.EntityFrameworkCore;
using NutritionTracker.Application.Ports.Output;
using NutritionTracker.Domain.Entities;
using NutritionTracker.SqlServer.Data;
using NutritionTracker.SqlServer.Mappers;

namespace NutritionTracker.SqlServer.Repositories;

public class FoodNutritionRepository : IFoodNutritionRepository
{
    private readonly NutritionTrackerDbContext _context;

    public FoodNutritionRepository(NutritionTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<FoodNutrition?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.FoodNutritions.FindAsync(new object[] { id }, cancellationToken);
        return entity != null ? EntityMapper.ToDomain(entity) : null;
    }

    public async Task<IEnumerable<FoodNutrition>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.FoodNutritions.ToListAsync(cancellationToken);
        return entities.Select(EntityMapper.ToDomain);
    }

    public async Task<IEnumerable<FoodNutrition>> SearchByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var entities = await _context.FoodNutritions
            .Where(fn => fn.Name.Contains(name))
            .ToListAsync(cancellationToken);
        return entities.Select(EntityMapper.ToDomain);
    }

    public async Task<FoodNutrition> AddAsync(FoodNutrition foodNutrition, CancellationToken cancellationToken = default)
    {
        var entity = EntityMapper.ToEntity(foodNutrition);
        _context.FoodNutritions.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return EntityMapper.ToDomain(entity);
    }

    public async Task<FoodNutrition> UpdateAsync(FoodNutrition foodNutrition, CancellationToken cancellationToken = default)
    {
        var entity = EntityMapper.ToEntity(foodNutrition);
        _context.FoodNutritions.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return EntityMapper.ToDomain(entity);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.FoodNutritions.FindAsync(new object[] { id }, cancellationToken);
        if (entity != null)
        {
            _context.FoodNutritions.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
