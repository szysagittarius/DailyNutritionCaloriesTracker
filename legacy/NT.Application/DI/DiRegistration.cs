using Microsoft.Extensions.DependencyInjection;
using NT.Application.Services.Abstractions;
using NT.Application.Services.Implementations;

namespace NT.Application.DI;
public static class DiRegistration
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IFoodLogService, FoodLogService>();
        services.AddScoped<IFoodNutritionService, FoodNutritionService>();
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
