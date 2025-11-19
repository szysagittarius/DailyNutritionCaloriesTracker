namespace NutritionTracker.Persistence.Contracts.Configuration;

/// <summary>
/// Base configuration interface for persistence settings
/// </summary>
public interface IPersistenceConfiguration
{
    string ConnectionString { get; }
    int CommandTimeout { get; }
    bool EnableSensitiveDataLogging { get; }
}

/// <summary>
/// SQL Server specific configuration
/// </summary>
public class SqlServerConfiguration : IPersistenceConfiguration
{
    public string ConnectionString { get; set; } = string.Empty;
    public int CommandTimeout { get; set; } = 30;
    public bool EnableSensitiveDataLogging { get; set; } = false;
    public bool EnableRetryOnFailure { get; set; } = true;
    public int MaxRetryCount { get; set; } = 3;
    public int MaxRetryDelay { get; set; } = 30;
}

/// <summary>
/// Azure Table Storage configuration
/// </summary>
public class AzureTableConfiguration : IPersistenceConfiguration
{
    public string ConnectionString { get; set; } = string.Empty;
    public int CommandTimeout { get; set; } = 30;
    public bool EnableSensitiveDataLogging { get; set; } = false;
    public string TableNamePrefix { get; set; } = string.Empty;
}

/// <summary>
/// NoSQL/Cosmos DB configuration
/// </summary>
public class CosmosDbConfiguration : IPersistenceConfiguration
{
    public string ConnectionString { get; set; } = string.Empty;
    public int CommandTimeout { get; set; } = 30;
    public bool EnableSensitiveDataLogging { get; set; } = false;
    public string DatabaseName { get; set; } = string.Empty;
    public string ContainerName { get; set; } = string.Empty;
    public int Throughput { get; set; } = 400;
}
