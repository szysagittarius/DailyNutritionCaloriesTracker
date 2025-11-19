using Microsoft.EntityFrameworkCore;
using NutritionTracker.Application.Ports.Output;
using NutritionTracker.Domain.Entities;
using NutritionTracker.SqlServer.Data;
using NutritionTracker.SqlServer.Mappers;

namespace NutritionTracker.SqlServer.Repositories;

public class FoodLogRepository : IFoodLogRepository
{
    private readonly NutritionTrackerDbContext _context;

    public FoodLogRepository(NutritionTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<FoodLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.FoodLogs
            .Include(fl => fl.FoodItems)
            .ThenInclude(fi => fi.FoodNutrition)
            .FirstOrDefaultAsync(fl => fl.Id == id, cancellationToken);

        return entity != null ? EntityMapper.ToDomain(entity) : null;
    }

    public async Task<IEnumerable<FoodLog>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.FoodLogs
            .Include(fl => fl.FoodItems)
            .ThenInclude(fi => fi.FoodNutrition)
            .ToListAsync(cancellationToken);

        return entities.Select(EntityMapper.ToDomain);
    }

    public async Task<IEnumerable<FoodLog>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var entities = await _context.FoodLogs
            .Include(fl => fl.FoodItems)
            .ThenInclude(fi => fi.FoodNutrition)
            .Where(fl => fl.UserId == userId)
            .OrderByDescending(fl => fl.DateTime)
            .ToListAsync(cancellationToken);

        return entities.Select(EntityMapper.ToDomain);
    }

    public async Task<FoodLog> AddAsync(FoodLog foodLog, CancellationToken cancellationToken = default)
    {
        var entity = EntityMapper.ToEntity(foodLog);
        _context.FoodLogs.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return EntityMapper.ToDomain(entity);
    }

    public async Task<FoodLog> UpdateAsync(FoodLog foodLog, CancellationToken cancellationToken = default)
    {
        var entity = EntityMapper.ToEntity(foodLog);
        _context.FoodLogs.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return EntityMapper.ToDomain(entity);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.FoodLogs.FindAsync(new object[] { id }, cancellationToken);
        if (entity != null)
        {
            _context.FoodLogs.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteAsync(FoodLog foodLog, CancellationToken cancellationToken = default)
    {
        await DeleteAsync(foodLog.Id, cancellationToken);
    }

    public async Task DeleteByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        var entities = await _context.FoodLogs.Where(fl => ids.Contains(fl.Id)).ToListAsync(cancellationToken);
        _context.FoodLogs.RemoveRange(entities);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAllAsync(CancellationToken cancellationToken = default)
    {
        await _context.Database.ExecuteSqlRawAsync("DELETE FROM FoodLogs", cancellationToken);
    }
}
