using AutoMapper;
using NT.Application.Contracts.Entities;
using NT.Application.Contracts.Ports;
using NT.Database.Entities;
using NT.Ef.Repositories.Abstractions;

namespace NT.Ef.Repositories.Implementations.DataHandlers;

internal class UserDataHandler : IUserDataHandler
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserDataHandler(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserEntity>> GetAllAsync()
    {
        List<User> users = await Task.FromResult(_userRepository.GetAll().ToList());
        return _mapper.Map<IEnumerable<UserEntity>>(users);
    }

    public Task<UserEntity> GetAsync(string username)
    {
        throw new NotImplementedException();
    }

    public async Task<UserEntity> UpdateAsync(UserEntity user)
    {
        User userModel = _mapper.Map<User>(user);
        User updatedUser = await _userRepository.UpdateAsync(userModel);
        return _mapper.Map<UserEntity>(updatedUser);
    }

    public async Task<UserEntity> AddAsync(UserEntity userEntity)
    {
        // Ensure the entity has an ID if it's new
        if (userEntity.Id == Guid.Empty)
        {
            userEntity.Id = Guid.NewGuid();
        }
        
        User user = _mapper.Map<User>(userEntity);
        User addedUser = await _userRepository.AddAsync(user);
        return _mapper.Map<UserEntity>(addedUser);
    }

    public Task DeleteAsync(string username)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(IEnumerable<string> usernames)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync()
    {
        throw new NotImplementedException();
    }
}
