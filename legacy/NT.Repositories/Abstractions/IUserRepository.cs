using NT.Database.Entities;

namespace NT.Ef.Repositories.Abstractions;

public interface IUserRepository : IBaseRepository<User>
{
    // No additional methods needed - inherits everything from IBaseRepository<User>
    // Including: GetAll(), AddAsync(), UpdateAsync(), GetByIdAsync(), DeleteAsync()
}