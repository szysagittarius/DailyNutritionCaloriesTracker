using AutoMapper;
using NT.Application.Contracts.Entities;
using NutritionTracker.Api.Models;

namespace NutritionTracker.Api.Profilers;

public class UserDtoProfiler : Profile
{
    public UserDtoProfiler()
    {
        CreateMap<UserEntity, UserDto>()
            .ForMember(dest => dest.SuggestedCalories, opt => opt.MapFrom(src => src.SuggestedCalories))
            .ForMember(dest => dest.SuggestedCarbs, opt => opt.MapFrom(src => src.SuggestedCarbs))
            .ForMember(dest => dest.SuggestedFat, opt => opt.MapFrom(src => src.SuggestedFat))
            .ForMember(dest => dest.SuggestedProtein, opt => opt.MapFrom(src => src.SuggestedProtein));

        CreateMap<UserDto, UserEntity>()
            .ForMember(dest => dest.SuggestedCalories, opt => opt.MapFrom(src => src.SuggestedCalories))
            .ForMember(dest => dest.SuggestedCarbs, opt => opt.MapFrom(src => src.SuggestedCarbs))
            .ForMember(dest => dest.SuggestedFat, opt => opt.MapFrom(src => src.SuggestedFat))
            .ForMember(dest => dest.SuggestedProtein, opt => opt.MapFrom(src => src.SuggestedProtein));
    }
}
