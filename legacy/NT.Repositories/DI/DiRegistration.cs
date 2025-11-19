using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using NT.Application.Contracts.Ports;
using NT.Ef.Repositories.Abstractions;
using NT.Ef.Repositories.Implementations;
using NT.Ef.Repositories.Implementations.DataHandlers;
using NT.Ef.Repositories.Implementations.Repositories;
using NT.Ef.Repositories.Profiler;

namespace NT.Ef.Repositories.DI;

public static class DiRegistration
{
    public static IServiceCollection RegisterRepository(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        //data Handlers
        services.AddScoped<IFoodNutritionDataHandler, FoodNutritionDataHandler>();
        services.AddScoped<IFoodLogDataHandler, FoodLogDataHandler>();
        //services.AddScoped<IFoodItemService, FoodItemDataHandler>();
        services.AddScoped<IUserDataHandler, UserDataHandler>();

        //repositories
        services.AddScoped<IFoodNutritionRepository, FoodNutritionRepository>();
        services.AddScoped<IFoodLogRepository, FoodLogRepository>();
        services.AddScoped<IFoodItemRepository, FoodItemRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        //profilers
        MapperConfiguration mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new FoodNutritionProfiler());
            cfg.AddProfile(new FoodItemProfiler());
            cfg.AddProfile(new FoodLogProfiler());
            cfg.AddProfile(new UserProfiler());
        });

        IMapper mapper = mapperConfig.CreateMapper();
        services.AddSingleton(mapper);

        return services;
    }
}
