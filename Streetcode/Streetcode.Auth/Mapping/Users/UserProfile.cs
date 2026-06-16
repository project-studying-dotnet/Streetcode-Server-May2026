using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Streetcode.Auth.Models.DTO;
using Streetcode.Auth.Models.Entities;
namespace Streetcode.Auth.Mapping.Users
{
    [ExcludeFromCodeCoverage]
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserLoginDto>();
            CreateMap<UserDto, UserLoginDto>().ReverseMap();
            CreateMap<User, UserDto>()
             .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
             .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
             .ForMember(dest => dest.Surname, opt => opt.MapFrom(src => src.Surname))
             .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
             .ForMember(dest => dest.Login, opt => opt.MapFrom(src => src.UserName))
             .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
             .ReverseMap();

            CreateMap<UserRegisterDto, User>()
             .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));
        }
    }
}
