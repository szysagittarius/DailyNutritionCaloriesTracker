namespace NT.Application.Contracts.Ports;

public interface IUnitOfWork : IDisposable
{
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
    Task SaveChangesAsync();
}

