using Azure.Data.Tables;
using Microsoft.Extensions.DependencyInjection;
using NutritionTracker.Application.Ports.Output;
using NutritionTracker.AzureTableStorage.Repositories;

namespace NutritionTracker.AzureTableStorage.DI;

public static class DependencyInjection
{
    public static IServiceCollection AddAzureTableStorage(
        this IServiceCollection services,
        string connectionString)
    {
        // Register TableServiceClient as singleton
        services.AddSingleton(sp => new TableServiceClient(connectionString));

        // Register repositories
        services.AddScoped<IUserRepository, UserTableRepository>();
        services.AddScoped<IFoodLogRepository, FoodLogTableRepository>();
        services.AddScoped<IFoodNutritionRepository, FoodNutritionTableRepository>();

        return services;
    }
}
