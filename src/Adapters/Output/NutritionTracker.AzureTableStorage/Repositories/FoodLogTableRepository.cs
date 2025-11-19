using Azure.Data.Tables;
using NutritionTracker.Application.Ports.Output;
using NutritionTracker.AzureTableStorage.Mappers;
using NutritionTracker.Domain.Entities;

namespace NutritionTracker.AzureTableStorage.Repositories;

public class FoodLogTableRepository : IFoodLogRepository
{
    private readonly TableClient _tableClient;

    public FoodLogTableRepository(TableServiceClient tableServiceClient)
    {
        _tableClient = tableServiceClient.GetTableClient("FoodLogs");
        _tableClient.CreateIfNotExists();
    }

    public async Task<FoodLog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Since RowKey includes timestamp, we need to query by Id property
        var query = _tableClient.QueryAsync<Entities.FoodLogTableEntity>(
            filter: $"Id eq '{id}'",
            cancellationToken: cancellationToken);

        await foreach (var entity in query)
        {
            return TableEntityMapper.ToDomain(entity);
        }

        return null;
    }

    public async Task<IEnumerable<FoodLog>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var foodLogs = new List<FoodLog>();
        var query = _tableClient.QueryAsync<Entities.FoodLogTableEntity>(
            cancellationToken: cancellationToken);

        await foreach (var entity in query)
        {
            foodLogs.Add(TableEntityMapper.ToDomain(entity));
        }

        return foodLogs;
    }

    public async Task<IEnumerable<FoodLog>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var foodLogs = new List<FoodLog>();
        // PartitionKey is UserId - highly efficient query!
        var query = _tableClient.QueryAsync<Entities.FoodLogTableEntity>(
            filter: $"PartitionKey eq '{userId}'",
            cancellationToken: cancellationToken);

        await foreach (var entity in query)
        {
            foodLogs.Add(TableEntityMapper.ToDomain(entity));
        }

        // Results are already sorted by RowKey (datetime_id), reverse for newest first
        return foodLogs.OrderByDescending(f => f.DateTime);
    }

    public async Task<FoodLog> AddAsync(FoodLog foodLog, CancellationToken cancellationToken = default)
    {
        var entity = TableEntityMapper.ToTableEntity(foodLog);
        await _tableClient.AddEntityAsync(entity, cancellationToken);
        return foodLog;
    }

    public async Task<FoodLog> UpdateAsync(FoodLog foodLog, CancellationToken cancellationToken = default)
    {
        // Delete old entity and add new one (since RowKey might change with DateTime update)
        var oldEntity = await GetExistingEntityAsync(foodLog.Id, cancellationToken);
        if (oldEntity != null)
        {
            await _tableClient.DeleteEntityAsync(oldEntity.PartitionKey, oldEntity.RowKey, cancellationToken: cancellationToken);
        }

        var newEntity = TableEntityMapper.ToTableEntity(foodLog);
        await _tableClient.AddEntityAsync(newEntity, cancellationToken);
        return foodLog;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetExistingEntityAsync(id, cancellationToken);
        if (entity != null)
        {
            await _tableClient.DeleteEntityAsync(entity.PartitionKey, entity.RowKey, cancellationToken: cancellationToken);
        }
    }

    public async Task DeleteAsync(FoodLog foodLog, CancellationToken cancellationToken = default)
    {
        await DeleteAsync(foodLog.Id, cancellationToken);
    }

    public async Task DeleteByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        foreach (var id in ids)
        {
            await DeleteAsync(id, cancellationToken);
        }
    }

    public async Task DeleteAllAsync(CancellationToken cancellationToken = default)
    {
        var query = _tableClient.QueryAsync<Entities.FoodLogTableEntity>(cancellationToken: cancellationToken);
        
        await foreach (var entity in query)
        {
            await _tableClient.DeleteEntityAsync(entity.PartitionKey, entity.RowKey, cancellationToken: cancellationToken);
        }
    }

    private async Task<Entities.FoodLogTableEntity?> GetExistingEntityAsync(Guid id, CancellationToken cancellationToken)
    {
        var query = _tableClient.QueryAsync<Entities.FoodLogTableEntity>(
            filter: $"Id eq '{id}'",
            cancellationToken: cancellationToken);

        await foreach (var entity in query)
        {
            return entity;
        }

        return null;
    }
}
