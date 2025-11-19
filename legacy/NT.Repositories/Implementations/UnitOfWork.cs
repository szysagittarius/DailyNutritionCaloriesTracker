using Microsoft.EntityFrameworkCore.Storage;
using NT.Application.Contracts.Ports;
using NT.Database.Context;

namespace NT.Ef.Repositories.Implementations;
public class UnitOfWork(NutritionTrackerDbContext context) : IUnitOfWork
{
    private readonly NutritionTrackerDbContext _context = (NutritionTrackerDbContext?)(context ?? throw new ArgumentNullException(nameof(context)));
    private IDbContextTransaction? _transaction;

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        try
        {
            await SaveChangesAsync();
            await _transaction?.CommitAsync();
        }
        catch
        {
            // Log exception, handle error as needed
            await RollbackAsync();
            throw;
        }
        finally
        {
            //await _transaction?.DisposeAsync();
            //_transaction = null;
        }
    }

    public async Task RollbackAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context?.Dispose();
    }
}