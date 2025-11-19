using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NutritionTracker.Application.Ports.Output;
using NutritionTracker.SqlServer.Data;
using NutritionTracker.SqlServer.Repositories;

namespace NutritionTracker.SqlServer;

public static class DependencyInjection
{
    public static IServiceCollection AddSqlServerPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<NutritionTrackerDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(NutritionTrackerDbContext).Assembly.FullName)));

        // Register repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IFoodLogRepository, FoodLogRepository>();
        services.AddScoped<IFoodNutritionRepository, FoodNutritionRepository>();

        return services;
    }
}
