using AutoMapper;
using SportsManagementApp.Data.DTOs.Auth;
using SportsManagementApp.Data.DTOs.UserManagement;
using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Mappings
{
    public class UserProfile: Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserResponseDto>()
                .ForMember(dest => dest.RoleName,
                    opt => opt.MapFrom(src => src.Role!.Name));

            CreateMap<CreateUserDto, User>();

            CreateMap<UpdateUserDto, User>()
                .ForAllMembers(opt =>
                    opt.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<User, LoginResponseDto>()
                .ForMember(dest => dest.Role,
                    opt => opt.MapFrom(src => src.Role!.Name));

        }
    }
}
