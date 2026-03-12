using AutoMapper;
using SportsManagementApp.Data.DTOs.Auth;
using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Mappings
{
    public class AuthProfile : Profile
    {
        public AuthProfile()
        {
            CreateMap<RegisterRequestDto, User>();

            CreateMap<User, LoginResponseDto>()
                .ForMember(dest => dest.Role,
                    opt => opt.MapFrom(src => src.Role!.Name));
        }
    }
}
