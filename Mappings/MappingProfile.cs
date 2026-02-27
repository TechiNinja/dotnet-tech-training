using AutoMapper;
using SportsManagementApp.DTOs.Response;
using SportsManagementApp.Entities;

namespace SportsManagementApp.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Event, EventResponse>()
                .ForMember(dest => dest.SportName,       opt => opt.MapFrom(src => src.Sport != null ? src.Sport.Name : string.Empty))
                .ForMember(dest => dest.OrganizerName,   opt => opt.MapFrom(src => src.Organizer != null ? src.Organizer.FullName : null))
                .ForMember(dest => dest.Status,          opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.TournamentType,  opt => opt.MapFrom(src => src.TournamentType.ToString()))
                .ForMember(dest => dest.Categories,      opt => opt.MapFrom(src => src.Categories));

            CreateMap<EventCategory, EventCategoryResponse>()
                .ForMember(dest => dest.Gender,  opt => opt.MapFrom(src => src.Gender.ToString()))
                .ForMember(dest => dest.Format,  opt => opt.MapFrom(src => src.Format.ToString()))
                .ForMember(dest => dest.Status,  opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<EventCategory, CategoryResponse>()
                .ForMember(dest => dest.Gender,              opt => opt.MapFrom(src => src.Gender.ToString()))
                .ForMember(dest => dest.Format,              opt => opt.MapFrom(src => src.Format.ToString()))
                .ForMember(dest => dest.Status,              opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.EventName,           opt => opt.MapFrom(src => src.Event != null ? src.Event.Name : string.Empty))
                .ForMember(dest => dest.TournamentType,      opt => opt.MapFrom(src => src.Event != null ? src.Event.TournamentType.ToString() : string.Empty))
                .ForMember(dest => dest.MaxParticipantsCount,opt => opt.MapFrom(src => src.Event != null ? src.Event.MaxParticipantsCount : 0));

            CreateMap<Match, FixtureResponse>()
                .ForMember(dest => dest.Status,       opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.IsBye,        opt => opt.MapFrom(src => src.SideAId == null || src.SideBId == null))
                .ForMember(dest => dest.SideAName,    opt => opt.MapFrom(src => string.Empty))
                .ForMember(dest => dest.SideBName,    opt => opt.MapFrom(src => string.Empty))
                .ForMember(dest => dest.Sets,         opt => opt.MapFrom(src => src.MatchSets))
                .ForMember(dest => dest.Result,       opt => opt.MapFrom(src => src.Result));

            CreateMap<MatchSet, MatchSetResponse>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<Result, MatchResultResponse>();
        }
    }
}