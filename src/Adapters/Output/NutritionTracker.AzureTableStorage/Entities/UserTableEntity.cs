using Azure;
using Azure.Data.Tables;

namespace NutritionTracker.AzureTableStorage.Entities;

/// <summary>
/// User entity for Azure Table Storage
/// PartitionKey: "USER" (all users in same partition for efficient queries)
/// RowKey: UserId (Guid)
/// </summary>
public class UserTableEntity : ITableEntity
{
    public string PartitionKey { get; set; } = "USER";
    public string RowKey { get; set; } = string.Empty; // UserId
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }

    // User properties
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public double SuggestedCalories { get; set; }
    public double SuggestedCarbs { get; set; }
    public double SuggestedFat { get; set; }
    public double SuggestedProtein { get; set; }
}
