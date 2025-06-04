using AutoMapper;
using SecondWebApi.Models.Dtos;
public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<PatientAddDto, User>()
            .ForMember(dest => dest.username, act => act.MapFrom(src => src.Email))
            .ForMember(dest => dest.password, opt => opt.Ignore());

        CreateMap<User, PatientAddDto>()
            .ForMember(dest => dest.Email, act => act.MapFrom(src => src.username));

        CreateMap<DoctorAddDto, User>()
            .ForMember(dest => dest.username, act => act.MapFrom(src => src.Email))
            .ForMember(dest => dest.password, opt => opt.Ignore());

        CreateMap<User, DoctorAddDto>()
             .ForMember(dest => dest.Email, act => act.MapFrom(src => src.username));

    }
}
