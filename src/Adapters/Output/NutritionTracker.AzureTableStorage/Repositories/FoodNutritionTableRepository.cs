using Azure.Data.Tables;
using NutritionTracker.Application.Ports.Output;
using NutritionTracker.AzureTableStorage.Mappers;
using NutritionTracker.Domain.Entities;

namespace NutritionTracker.AzureTableStorage.Repositories;

public class FoodNutritionTableRepository : IFoodNutritionRepository
{
    private readonly TableClient _tableClient;

    public FoodNutritionTableRepository(TableServiceClient tableServiceClient)
    {
        _tableClient = tableServiceClient.GetTableClient("FoodNutritions");
        _tableClient.CreateIfNotExists();
    }

    public async Task<FoodNutrition?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _tableClient.GetEntityAsync<Entities.FoodNutritionTableEntity>(
                "FOOD",
                id.ToString(),
                cancellationToken: cancellationToken);

            return TableEntityMapper.ToDomain(response.Value);
        }
        catch (Azure.RequestFailedException ex) when (ex.Status == 404)
        {
            return null;
        }
    }

    public async Task<IEnumerable<FoodNutrition>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var foodNutritions = new List<FoodNutrition>();
        var query = _tableClient.QueryAsync<Entities.FoodNutritionTableEntity>(
            filter: "PartitionKey eq 'FOOD'",
            cancellationToken: cancellationToken);

        await foreach (var entity in query)
        {
            foodNutritions.Add(TableEntityMapper.ToDomain(entity));
        }

        return foodNutritions;
    }

    public async Task<IEnumerable<FoodNutrition>> SearchByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var foodNutritions = new List<FoodNutrition>();
        var query = _tableClient.QueryAsync<Entities.FoodNutritionTableEntity>(
            filter: $"PartitionKey eq 'FOOD'",
            cancellationToken: cancellationToken);

        await foreach (var entity in query)
        {
            if (entity.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
            {
                foodNutritions.Add(TableEntityMapper.ToDomain(entity));
            }
        }

        return foodNutritions;
    }

    public async Task<FoodNutrition> AddAsync(FoodNutrition foodNutrition, CancellationToken cancellationToken = default)
    {
        var entity = TableEntityMapper.ToTableEntity(foodNutrition);
        await _tableClient.AddEntityAsync(entity, cancellationToken);
        return foodNutrition;
    }

    public async Task<FoodNutrition> UpdateAsync(FoodNutrition foodNutrition, CancellationToken cancellationToken = default)
    {
        var entity = TableEntityMapper.ToTableEntity(foodNutrition);
        await _tableClient.UpdateEntityAsync(entity, Azure.ETag.All, cancellationToken: cancellationToken);
        return foodNutrition;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _tableClient.DeleteEntityAsync("FOOD", id.ToString(), cancellationToken: cancellationToken);
    }
}
