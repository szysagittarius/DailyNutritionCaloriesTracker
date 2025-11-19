using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NT.Database.Context;

namespace NT.Database.DI;

public static class DiRegistration
{
    public static IServiceCollection RegisterEfDatabase(this IServiceCollection services, 
        Action<DatabaseConfig> configBuilder
        , ILogger logger)
    {
        // Log the entry to the function and any received arguments
        logger.LogInformation("RegisterEfDatabase started.");

        ArgumentNullException.ThrowIfNull(configBuilder, nameof(configBuilder));

        DatabaseConfig dbConfig = new DatabaseConfig();
        configBuilder(dbConfig);

        // Log to check if the config value was set properly
        if (string.IsNullOrEmpty(dbConfig.ConnectionString))
        {
            logger.LogError("from db level, Database connection string is null or empty.");
        }
        else
        {
            logger.LogInformation("from db level, Database connection string is: {ConnectionString}", dbConfig.ConnectionString);
        }


        ArgumentException.ThrowIfNullOrEmpty(dbConfig.ConnectionString, nameof(dbConfig.ConnectionString));

        services.AddDbContext<NutritionTrackerDbContext>(opt => opt.UseSqlServer(dbConfig.ConnectionString));

        return services;
    }
}
