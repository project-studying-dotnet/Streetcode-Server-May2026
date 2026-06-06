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
            CreateMap<User, UserDto>().ReverseMap();

            CreateMap<UserRegisterDto, User>();
        }
    }
}
