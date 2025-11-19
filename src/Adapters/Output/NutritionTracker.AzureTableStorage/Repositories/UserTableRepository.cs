using Azure.Data.Tables;
using NutritionTracker.Application.Ports.Output;
using NutritionTracker.AzureTableStorage.Mappers;
using NutritionTracker.Domain.Entities;

namespace NutritionTracker.AzureTableStorage.Repositories;

public class UserTableRepository : IUserRepository
{
    private readonly TableClient _tableClient;

    public UserTableRepository(TableServiceClient tableServiceClient)
    {
        _tableClient = tableServiceClient.GetTableClient("Users");
        _tableClient.CreateIfNotExists();
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _tableClient.GetEntityAsync<Entities.UserTableEntity>(
                "USER", 
                id.ToString(), 
                cancellationToken: cancellationToken);
            
            return TableEntityMapper.ToDomain(response.Value);
        }
        catch (Azure.RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var query = _tableClient.QueryAsync<Entities.UserTableEntity>(
            filter: $"PartitionKey eq 'USER' and Email eq '{email}'",
            cancellationToken: cancellationToken);

        await foreach (var entity in query)
        {
            return TableEntityMapper.ToDomain(entity);
        }

        return null;
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        var query = _tableClient.QueryAsync<Entities.UserTableEntity>(
            filter: $"PartitionKey eq 'USER' and Name eq '{username}'",
            cancellationToken: cancellationToken);

        await foreach (var entity in query)
        {
            return TableEntityMapper.ToDomain(entity);
        }

        return null;
    }

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = new List<User>();
        var query = _tableClient.QueryAsync<Entities.UserTableEntity>(
            filter: "PartitionKey eq 'USER'",
            cancellationToken: cancellationToken);

        await foreach (var entity in query)
        {
            users.Add(TableEntityMapper.ToDomain(entity));
        }

        return users;
    }

    public async Task<User> AddAsync(User user, CancellationToken cancellationToken = default)
    {
        var entity = TableEntityMapper.ToTableEntity(user);
        await _tableClient.AddEntityAsync(entity, cancellationToken);
        return user;
    }

    public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        var entity = TableEntityMapper.ToTableEntity(user);
        await _tableClient.UpdateEntityAsync(entity, Azure.ETag.All, cancellationToken: cancellationToken);
        return user;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _tableClient.DeleteEntityAsync("USER", id.ToString(), cancellationToken: cancellationToken);
    }
}
