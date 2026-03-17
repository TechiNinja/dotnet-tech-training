using AutoMapper;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Enums;
using SportsManagementApp.DTOs.Fixture;

namespace SportsManagementApp.Mappings
{
    public class MatchMapping : Profile
    {
        public MatchMapping()
        {
            CreateMap<EventCategory, EventCategoryResponseDto>()
                .ForMember(d => d.Gender, opt => opt.MapFrom(s => s.Gender.ToString()))
                .ForMember(d => d.Format, opt => opt.MapFrom(s => s.Format.ToString()))
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));

            CreateMap<Match, FixtureResponseDto>()
                .ForMember(d => d.Status,    opt => opt.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.IsBye,     opt => opt.MapFrom(s => s.SideAId == null || s.SideBId == null))
                .ForMember(d => d.SideAName, opt => opt.MapFrom(_ => string.Empty))
                .ForMember(d => d.SideBName, opt => opt.MapFrom(_ => string.Empty))
                .ForMember(d => d.Sets,      opt => opt.MapFrom(s => s.MatchSets))
                .ForMember(d => d.Result,    opt => opt.MapFrom(s => s.Result));

            CreateMap<MatchSet, MatchSetResponseDto>()
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));

            CreateMap<Result, MatchResultResponseDto>();

            CreateMap<MatchSetRequestDto, MatchSet>()
                .ForMember(d => d.Id,        opt => opt.Ignore())
                .ForMember(d => d.MatchId,   opt => opt.Ignore())
                .ForMember(d => d.SetNumber, opt => opt.Ignore())
                .ForMember(d => d.Status,    opt => opt.Ignore())
                .ForMember(d => d.CreatedAt, opt => opt.Ignore())
                .ForMember(d => d.UpdatedAt, opt => opt.Ignore())
                .ForMember(d => d.Match,     opt => opt.Ignore());
        }
    }
}