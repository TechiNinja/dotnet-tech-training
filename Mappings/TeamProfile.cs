using AutoMapper;
using SportsManagementApp.Data.DTOs.TeamManagement;
using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Mappings
{
    public class TeamProfile : Profile
    {
        public TeamProfile()
        {
            CreateMap<Team, TeamResponseDto>()
                .ForMember(dest => dest.Members,
                     opt => opt.MapFrom(src => src.Members.Select(member => member.User!.FullName)));

            CreateMap<CreateTeamRequestDto, Team>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Members, opt => opt.Ignore());
        }
    }
}
