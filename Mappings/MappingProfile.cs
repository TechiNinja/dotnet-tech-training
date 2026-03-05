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
                .ForMember(d => d.SportName,      opt => opt.MapFrom(s => s.Sport != null ? s.Sport.Name : string.Empty))
                .ForMember(d => d.OrganizerName,  opt => opt.MapFrom(s => s.Organizer != null ? s.Organizer.FullName : null))
                .ForMember(d => d.Status,         opt => opt.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.TournamentType, opt => opt.MapFrom(s => s.TournamentType.ToString()))
                .ForMember(d => d.Categories,     opt => opt.MapFrom(s => s.Categories));

            CreateMap<EventCategory, EventCategoryResponse>()
                .ForMember(d => d.Gender, opt => opt.MapFrom(s => s.Gender.ToString()))
                .ForMember(d => d.Format, opt => opt.MapFrom(s => s.Format.ToString()))
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));

            CreateMap<EventCategory, CategoryResponse>()
                .ForMember(d => d.Gender,               opt => opt.MapFrom(s => s.Gender.ToString()))
                .ForMember(d => d.Format,               opt => opt.MapFrom(s => s.Format.ToString()))
                .ForMember(d => d.Status,               opt => opt.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.EventName,            opt => opt.MapFrom(s => s.Event != null ? s.Event.Name : string.Empty))
                .ForMember(d => d.TournamentType,       opt => opt.MapFrom(s => s.Event != null ? s.Event.TournamentType.ToString() : string.Empty))
                .ForMember(d => d.MaxParticipantsCount, opt => opt.MapFrom(s => s.Event != null ? s.Event.MaxParticipantsCount : 0));

            CreateMap<Match, FixtureResponse>()
                .ForMember(d => d.Status,    opt => opt.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.IsBye,     opt => opt.MapFrom(s => s.SideAId == null || s.SideBId == null))
                .ForMember(d => d.SideAName, opt => opt.MapFrom(_ => string.Empty))
                .ForMember(d => d.SideBName, opt => opt.MapFrom(_ => string.Empty))
                .ForMember(d => d.Sets,      opt => opt.MapFrom(s => s.MatchSets))
                .ForMember(d => d.Result,    opt => opt.MapFrom(s => s.Result));

            CreateMap<MatchSet, MatchSetResponse>()
                .ForMember(d => d.Status, opt => opt.MapFrom(s => s.Status.ToString()));

            CreateMap<Result, MatchResultResponse>();
        }
    }
}
