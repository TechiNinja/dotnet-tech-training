using AutoMapper;
using SportsManagementApp.DTOs.Request;
using SportsManagementApp.DTOs.Response;
using SportsManagementApp.Data.Entities;

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

            CreateMap<MatchSetRequest, MatchSet>()
                .ForMember(d => d.ScoreA, opt => opt.MapFrom(s => s.ScoreA))
                .ForMember(d => d.ScoreB, opt => opt.MapFrom(s => s.ScoreB))
                .ForMember(d => d.Id,        opt => opt.Ignore())
                .ForMember(d => d.MatchId,   opt => opt.Ignore())
                .ForMember(d => d.SetNumber, opt => opt.Ignore())
                .ForMember(d => d.Status,    opt => opt.Ignore())
                .ForMember(d => d.CreatedAt, opt => opt.Ignore())
                .ForMember(d => d.UpdatedAt, opt => opt.Ignore())
                .ForMember(d => d.Match,     opt => opt.Ignore());

            CreateMap<MatchScheduleItem, Match>()
                .ForMember(d => d.MatchDateTime, opt => opt.MapFrom(s => s.MatchDateTime))
                .ForMember(d => d.TotalSets,     opt => opt.MapFrom(s => s.TotalSets))
                .ForMember(d => d.MatchVenue,    opt => opt.Ignore())
                .ForMember(d => d.Status,        opt => opt.Ignore())
                .ForMember(d => d.SideAId,       opt => opt.Ignore())
                .ForMember(d => d.SideBId,       opt => opt.Ignore())
                .ForMember(d => d.MatchSets,     opt => opt.Ignore())
                .ForMember(d => d.Result,        opt => opt.Ignore())
                .ForMember(d => d.CreatedAt,     opt => opt.Ignore())
                .ForMember(d => d.UpdatedAt,     opt => opt.Ignore())
                .ForMember(d => d.EventCategory, opt => opt.Ignore());

            CreateMap<CreateEventRequest, Event>()
                .ForMember(d => d.EventRequestId,       opt => opt.MapFrom(s => s.EventRequestId))
                .ForMember(d => d.Description,          opt => opt.MapFrom(s => s.Description))
                .ForMember(d => d.MaxParticipantsCount, opt => opt.MapFrom(s => s.MaxParticipantsCount))
                .ForMember(d => d.RegistrationDeadline, opt => opt.MapFrom(s => s.RegistrationDeadline))
                .ForMember(d => d.Name,          opt => opt.Ignore())
                .ForMember(d => d.SportId,       opt => opt.Ignore())
                .ForMember(d => d.StartDate,     opt => opt.Ignore())
                .ForMember(d => d.EndDate,       opt => opt.Ignore())
                .ForMember(d => d.EventVenue,    opt => opt.Ignore())
                .ForMember(d => d.Status,        opt => opt.Ignore())
                .ForMember(d => d.OrganizerId,   opt => opt.Ignore())
                .ForMember(d => d.TournamentType,opt => opt.Ignore())
                .ForMember(d => d.Categories,    opt => opt.Ignore())
                .ForMember(d => d.Sport,         opt => opt.Ignore())
                .ForMember(d => d.Organizer,     opt => opt.Ignore());

            CreateMap<PatchEventRequest, Event>()
                .ForMember(d => d.Name,
                    opt => { opt.Condition(s => s.Name != null); opt.MapFrom(s => s.Name); })
                .ForMember(d => d.Description,
                    opt => { opt.Condition(s => s.Description != null); opt.MapFrom(s => s.Description); })
                .ForMember(d => d.MaxParticipantsCount,
                    opt => { opt.Condition(s => s.MaxParticipantsCount.HasValue); opt.MapFrom(s => s.MaxParticipantsCount); })
                .ForMember(d => d.RegistrationDeadline,
                    opt => { opt.Condition(s => s.RegistrationDeadline.HasValue); opt.MapFrom(s => s.RegistrationDeadline); })
                .ForMember(d => d.SportId,        opt => opt.Ignore())
                .ForMember(d => d.StartDate,      opt => opt.Ignore())
                .ForMember(d => d.EndDate,        opt => opt.Ignore())
                .ForMember(d => d.EventVenue,     opt => opt.Ignore())
                .ForMember(d => d.Status,         opt => opt.Ignore())
                .ForMember(d => d.OrganizerId,    opt => opt.Ignore())
                .ForMember(d => d.TournamentType, opt => opt.Ignore())
                .ForMember(d => d.EventRequestId, opt => opt.Ignore())
                .ForMember(d => d.Categories,     opt => opt.Ignore())
                .ForMember(d => d.Sport,          opt => opt.Ignore())
                .ForMember(d => d.Organizer,      opt => opt.Ignore());

            CreateMap<EventRequest, EventRequestPreFillResponse>()
                .ForMember(d => d.SportName,             opt => opt.MapFrom(s => s.Sport != null ? s.Sport.Name : string.Empty))
                .ForMember(d => d.Gender,                opt => opt.MapFrom(s => s.Gender.ToString()))
                .ForMember(d => d.Format,                opt => opt.MapFrom(s => s.Format.ToString()))
                .ForMember(d => d.Status,                opt => opt.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.Name,                  opt => opt.MapFrom(s => s.EventName))
                .ForMember(d => d.Description,           opt => opt.MapFrom(_ => (string?)null))
                .ForMember(d => d.RegistrationDeadline,  opt => opt.MapFrom(_ => (DateTime?)null))
                .ForMember(d => d.IsEventAlreadyCreated, opt => opt.Ignore());
        }
    }
}