using NT.Application.Contracts.Entities;

namespace NT.Application.Services.Abstractions;

public interface IUserService
{
    Task<IEnumerable<UserEntity>> GetAllAsync();
    Task<UserEntity> AddAsync(UserEntity entity);
    Task<UserEntity> UpdateAsync(UserEntity entity);
}
