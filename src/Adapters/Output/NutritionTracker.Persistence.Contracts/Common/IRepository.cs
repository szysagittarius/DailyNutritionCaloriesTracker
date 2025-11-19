namespace NutritionTracker.Persistence.Contracts.Common;

/// <summary>
/// Base interface for all repository implementations
/// Provides common CRUD operations
/// </summary>
public interface IRepository<TEntity, TKey> where TEntity : class
{
    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(TKey id, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository with batch operations support
/// </summary>
public interface IBatchRepository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class
{
    Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    Task DeleteRangeAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default);
}

/// <summary>
/// Repository with query capabilities
/// </summary>
public interface IQueryableRepository<TEntity, TKey> : IRepository<TEntity, TKey> where TEntity : class
{
    Task<IEnumerable<TEntity>> FindAsync(
        System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);
    
    Task<TEntity?> FirstOrDefaultAsync(
        System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);
    
    Task<bool> ExistsAsync(
        System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default);
    
    Task<int> CountAsync(
        System.Linq.Expressions.Expression<Func<TEntity, bool>>? predicate = null,
        CancellationToken cancellationToken = default);
}
