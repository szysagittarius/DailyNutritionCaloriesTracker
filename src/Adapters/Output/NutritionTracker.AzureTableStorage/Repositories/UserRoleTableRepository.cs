using Azure;
using Azure.Data.Tables;
using NutritionTracker.Application.Ports.Output;
using NutritionTracker.Domain.Entities;

namespace NutritionTracker.AzureTableStorage.Repositories;

public class UserRoleTableRepository : IUserRoleRepository
{
    private readonly TableClient _tableClient;

    public UserRoleTableRepository(TableServiceClient tableServiceClient)
    {
        _tableClient = tableServiceClient.GetTableClient("UserRoles");
    }

    public async Task<IEnumerable<UserRole>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var roles = new List<UserRole>();
        var query = _tableClient.QueryAsync<Entities.UserRoleTableEntity>(
            filter: "PartitionKey eq 'ROLE'",
            cancellationToken: cancellationToken);

        await foreach (var entity in query)
        {
            roles.Add(new UserRole(Guid.Parse(entity.RowKey), entity.Name));
        }

        return roles.OrderBy(r => r.Name);
    }

    public async Task<UserRole?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _tableClient.GetEntityAsync<Entities.UserRoleTableEntity>(
                "ROLE",
                id.ToString(),
                cancellationToken: cancellationToken);

            return new UserRole(Guid.Parse(response.Value.RowKey), response.Value.Name);
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    public async Task<UserRole?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var query = _tableClient.QueryAsync<Entities.UserRoleTableEntity>(
            filter: $"PartitionKey eq 'ROLE' and Name eq '{name}'",
            cancellationToken: cancellationToken);

        await foreach (var entity in query)
        {
            return new UserRole(Guid.Parse(entity.RowKey), entity.Name);
        }

        return null;
    }

    public async Task<UserRole> AddAsync(UserRole role, CancellationToken cancellationToken = default)
    {
        await _tableClient.CreateIfNotExistsAsync(cancellationToken);

        var entity = new Entities.UserRoleTableEntity
        {
            PartitionKey = "ROLE",
            RowKey = role.Id.ToString(),
            Name = role.Name
        };

        await _tableClient.AddEntityAsync(entity, cancellationToken);
        return role;
    }

    public async Task EnsureDefaultRolesAsync(CancellationToken cancellationToken = default)
    {
        await _tableClient.CreateIfNotExistsAsync(cancellationToken);

        var roles = new[]
        {
            UserRole.User,
            UserRole.Admin
        };

        foreach (var role in roles)
        {
            var existing = await GetByIdAsync(role.Id, cancellationToken);
            if (existing == null)
            {
                await AddAsync(role, cancellationToken);
            }
        }
    }
}
