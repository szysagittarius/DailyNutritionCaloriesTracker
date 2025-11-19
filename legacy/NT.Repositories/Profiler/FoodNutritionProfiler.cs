using AutoMapper;
using NT.Application.Contracts.Entities;
using NT.Database.Entities;

namespace NT.Ef.Repositories.Profiler;
internal class FoodNutritionProfiler : Profile
{
    public FoodNutritionProfiler()
    {
        CreateMap<FoodNutritionEntity, FoodNutrition>().ReverseMap();
    }
}
