using NT.Database.Context;
using NT.Ef.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace NT.Ef.Repositories.Implementations.Repositories;

public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
{
    protected readonly NutritionTrackerDbContext dbContext;

    public BaseRepository(NutritionTrackerDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        try
        {
            await dbContext.Set<TEntity>().AddAsync(entity);
            await dbContext.SaveChangesAsync();
            return entity;
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        TEntity? entity = await dbContext.Set<TEntity>().FindAsync(id);
        if (entity != null)
        {
            dbContext.Set<TEntity>().Remove(entity);
            await dbContext.SaveChangesAsync();
        }
    }

    public IQueryable<TEntity> GetAll()
    {
        return dbContext.Set<TEntity>().AsQueryable();
    }

    public async Task<TEntity> GetByIdAsync(Guid id)
    {
        return await dbContext.Set<TEntity>().FindAsync(id);
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        try
        {
            var entry = dbContext.Entry(entity);
            
            // If entity is already tracked and modified, just save
            if (entry.State == EntityState.Modified)
            {
                await dbContext.SaveChangesAsync();
                return entity;
            }
            
            // If entity is tracked but not modified, mark as modified
            if (entry.State != EntityState.Detached)
            {
                entry.State = EntityState.Modified;
                await dbContext.SaveChangesAsync();
                return entity;
            }
            
            // For detached entities, check for tracking conflicts and resolve them
            var entityType = dbContext.Model.FindEntityType(typeof(TEntity));
            var key = entityType?.FindPrimaryKey();
            
            if (key != null && key.Properties.Count > 0)
            {
                var keyProperty = key.Properties[0];
                var keyValue = keyProperty.PropertyInfo?.GetValue(entity);
                
                if (keyValue != null)
                {
                    // Find any existing tracked entity with the same key
                    var trackedEntity = dbContext.ChangeTracker.Entries<TEntity>()
                        .FirstOrDefault(e => e.State != EntityState.Detached && 
                                           keyProperty.PropertyInfo?.GetValue(e.Entity)?.Equals(keyValue) == true);
                    
                    if (trackedEntity != null)
                    {
                        // Update the existing tracked entity instead of creating a conflict
                        trackedEntity.CurrentValues.SetValues(entity);
                        trackedEntity.State = EntityState.Modified;
                        await dbContext.SaveChangesAsync();
                        return trackedEntity.Entity;
                    }
                }
            }
            
            // If no conflict, proceed with normal update
            dbContext.Set<TEntity>().Update(entity);
            await dbContext.SaveChangesAsync();
            return entity;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error updating entity: {ex.Message}", ex);
        }
    }
}

