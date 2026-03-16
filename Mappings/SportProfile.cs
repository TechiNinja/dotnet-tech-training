using AutoMapper;
using SportsManagementApp.Data.DTOs.SportManagement;
using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Mappings
{
    public class SportProfile: Profile
    {
        public SportProfile()
        {
            CreateMap<CreateSportDto, Sport>();
            CreateMap<Sport, SportResponseDto>();
        }
    }
}
