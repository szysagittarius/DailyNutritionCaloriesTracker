using AutoMapper;
using NT.Application.Contracts.Entities;
using NT.Database.Entities;

namespace NT.Ef.Repositories.Profiler;
internal class FoodItemProfiler : Profile
{
    public FoodItemProfiler()
    {
        CreateMap<FoodItemEntity, FoodItem>().ReverseMap();
    }
}
