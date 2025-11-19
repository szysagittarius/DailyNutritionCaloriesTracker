using Microsoft.Extensions.DependencyInjection;
using NutritionTracker.Application.UseCases.FoodLogs;
using NutritionTracker.Application.UseCases.Nutrition;
using NutritionTracker.Application.UseCases.Users;

namespace NutritionTracker.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register FoodLog use cases
        services.AddScoped<ICreateFoodLogUseCase, CreateFoodLogUseCase>();
        services.AddScoped<IGetFoodLogsByUserUseCase, GetFoodLogsByUserUseCase>();
        services.AddScoped<GetAllFoodLogsUseCase>();
        services.AddScoped<UpdateFoodLogUseCase>();
        services.AddScoped<DeleteFoodLogUseCase>();

        // Register User use cases
        services.AddScoped<GetAllUsersUseCase>();
        services.AddScoped<GetUserByIdUseCase>();
        services.AddScoped<GetUserByUsernameUseCase>();
        services.AddScoped<CreateUserUseCase>();
        services.AddScoped<UpdateUserUseCase>();

        // Register FoodNutrition use cases
        services.AddScoped<GetAllFoodNutritionUseCase>();
        services.AddScoped<CreateFoodNutritionUseCase>();

        return services;
    }
}

