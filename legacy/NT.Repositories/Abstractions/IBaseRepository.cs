namespace NT.Ef.Repositories.Abstractions;

public interface IBaseRepository<TEntity> where TEntity : class
{
    IQueryable<TEntity> GetAll();
    Task<TEntity> GetByIdAsync(Guid id);
    Task<TEntity> AddAsync(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity entity);
    Task DeleteAsync(Guid id);
}
