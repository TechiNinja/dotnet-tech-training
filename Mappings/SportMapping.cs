using AutoMapper;
using SportsManagementApp.Data.DTOs.SportManagement;
using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Mappings;

public class SportMapping : Profile
{
    public SportMapping()
    {
        CreateMap<CreateSportDto, Sport>()
            .ForMember(dest => dest.AllowedFormats,
                opt => opt.MapFrom(src => src.AllowedFormats ?? new List<string>()))
            .ForMember(dest => dest.CreatedAt,
                opt => opt.MapFrom(_ => DateTime.UtcNow));

        CreateMap<UpdateSportDto, Sport>()
            .ForMember(dest => dest.AllowedFormats,
                opt => opt.MapFrom(src => src.AllowedFormats ?? new List<string>()))
            .ForMember(dest => dest.UpdatedAt,
                opt => opt.MapFrom(_ => DateTime.UtcNow));

        CreateMap<Sport, SportResponseDto>();
    }
}