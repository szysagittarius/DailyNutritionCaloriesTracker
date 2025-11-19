using Microsoft.EntityFrameworkCore;
using NutritionTracker.Application.Ports.Output;
using NutritionTracker.Domain.Entities;
using NutritionTracker.SqlServer.Data;
using NutritionTracker.SqlServer.Mappers;

namespace NutritionTracker.SqlServer.Repositories;

public class UserRepository : IUserRepository
{
    private readonly NutritionTrackerDbContext _context;

    public UserRepository(NutritionTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Users.FindAsync(new object[] { id }, cancellationToken);
        return entity != null ? EntityMapper.ToDomain(entity) : null;
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        return entity != null ? EntityMapper.ToDomain(entity) : null;
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Users.FirstOrDefaultAsync(u => u.Name == username, cancellationToken);
        return entity != null ? EntityMapper.ToDomain(entity) : null;
    }

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _context.Users.ToListAsync(cancellationToken);
        return entities.Select(EntityMapper.ToDomain);
    }

    public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        var entity = EntityMapper.ToEntity(user);
        _context.Users.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return EntityMapper.ToDomain(entity);
    }

    public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        var entity = EntityMapper.ToEntity(user);
        _context.Users.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return EntityMapper.ToDomain(entity);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _context.Users.FindAsync(new object[] { id }, cancellationToken);
        if (entity != null)
        {
            _context.Users.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
