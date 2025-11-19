using AutoMapper;
using NT.Application.Contracts.Entities;
using NT.Database.Entities;

namespace NT.Ef.Repositories.Profiler;
internal class UserProfiler : Profile
{
    public UserProfiler()
    {
        CreateMap<UserEntity, User>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
            .ForMember(dest => dest.SuggestedCalories, opt => opt.MapFrom(src => src.SuggestedCalories))
            .ForMember(dest => dest.SuggestedCarbs, opt => opt.MapFrom(src => src.SuggestedCarbs))
            .ForMember(dest => dest.SuggestedFat, opt => opt.MapFrom(src => src.SuggestedFat))
            .ForMember(dest => dest.SuggestedProtein, opt => opt.MapFrom(src => src.SuggestedProtein));

        CreateMap<User, UserEntity>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
            .ForMember(dest => dest.SuggestedCalories, opt => opt.MapFrom(src => src.SuggestedCalories))
            .ForMember(dest => dest.SuggestedCarbs, opt => opt.MapFrom(src => src.SuggestedCarbs))
            .ForMember(dest => dest.SuggestedFat, opt => opt.MapFrom(src => src.SuggestedFat))
            .ForMember(dest => dest.SuggestedProtein, opt => opt.MapFrom(src => src.SuggestedProtein));
    }
}
