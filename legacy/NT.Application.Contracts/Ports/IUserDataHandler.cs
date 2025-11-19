using NT.Application.Contracts.Entities;

namespace NT.Application.Contracts.Ports;

public interface IUserDataHandler
{
    Task<IEnumerable<UserEntity>> GetAllAsync();
    Task<UserEntity> GetAsync(string username);
    Task<UserEntity> AddAsync(UserEntity userEntity);
    Task<UserEntity> UpdateAsync(UserEntity user);
    Task DeleteAsync(string username);
    Task DeleteAsync(IEnumerable<string> usernames);
    Task DeleteAsync();
}
