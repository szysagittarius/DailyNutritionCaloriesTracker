using AutoMapper;
using NT.Application.Contracts.Entities;
using NutritionTracker.Api.Models;

namespace NutritionTracker.Api.Profilers;

public class FoodLogDtoProfiler : Profile
{
    public FoodLogDtoProfiler()
    {
        CreateMap<FoodLogDto, FoodLogEntity>()
                       .ForMember(dest => dest.FoodItems, opt => opt.MapFrom(src => src.FoodItems))
                       .ReverseMap();

        //CreateMap<FoodLogDto, FoodLogEntity>()
        //    .ForMember(dest => dest.DateTime, opt => opt.MapFrom(src => src.DateTime))
        //    .ForMember(dest => dest.CreateTime, opt => opt.MapFrom(src => src.CreateTime))
        //    .ForMember(dest => dest.UpdateTime, opt => opt.MapFrom(src => src.UpdateTime))
        //    .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
        //    .ForMember(dest => dest.FoodItems, opt => opt.MapFrom(src => src.FoodItems)); // This assumes you have similar mapping for FoodItemDto to FoodItemEntity
    }
}
