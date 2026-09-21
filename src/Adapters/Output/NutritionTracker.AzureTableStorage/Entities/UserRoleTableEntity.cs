using Azure;
using Azure.Data.Tables;

namespace NutritionTracker.AzureTableStorage.Entities;

public class UserRoleTableEntity : ITableEntity
{
    public string PartitionKey { get; set; } = "ROLE";
    public string RowKey { get; set; } = string.Empty;
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    public string Name { get; set; } = string.Empty;
}
