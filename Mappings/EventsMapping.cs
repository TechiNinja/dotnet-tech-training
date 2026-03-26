using AutoMapper;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.DTOs.EventCreation;

namespace SportsManagementApp.Mappings
{
    public class EventsMapping : Profile
    {
        public EventsMapping()
        {
            CreateMap<Event, EventResponseDto>()
                .ForMember(dest => dest.SportName,
                    opt => opt.MapFrom(src => src.Sport != null ? src.Sport.Name : string.Empty))
                .ForMember(dest => dest.OrganizerName,
                    opt => opt.MapFrom(src => src.Organizer != null ? src.Organizer.FullName : null))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.TournamentType,
                    opt => opt.MapFrom(src => src.TournamentType.ToString()));

            CreateMap<EventCategory, EventCategoryResponseDto>()
                .ForMember(dest => dest.Gender, opt => opt.MapFrom(src => src.Gender.ToString()))
                .ForMember(dest => dest.Format, opt => opt.MapFrom(src => src.Format.ToString()))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<EventRequest, EventRequestPreFillResponseDto>()
                .ForMember(dest => dest.SportName,
                    opt => opt.MapFrom(src => src.Sport != null ? src.Sport.Name : string.Empty))
                .ForMember(dest => dest.Gender,
                    opt => opt.MapFrom(src => src.Gender.ToString()))
                .ForMember(dest => dest.Format,
                    opt => opt.MapFrom(src => src.Format.ToString()))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Name,
                    opt => opt.MapFrom(src => src.EventName))
                .ForMember(dest => dest.IsEventAlreadyCreated,
                    opt => opt.Ignore());

            CreateMap<PatchEventRequestDto, Event>()
                .ForMember(dest => dest.Name,
                    opt => { opt.Condition(src => src.Name != null); opt.MapFrom(src => src.Name); })
                .ForMember(dest => dest.Description,
                    opt => { opt.Condition(src => src.Description != null); opt.MapFrom(src => src.Description); })
                .ForMember(dest => dest.MaxParticipantsCount,
                    opt => { opt.Condition(src => src.MaxParticipantsCount.HasValue); opt.MapFrom(src => src.MaxParticipantsCount); })
                .ForMember(dest => dest.RegistrationDeadline,
                    opt => { opt.Condition(src => src.RegistrationDeadline.HasValue); opt.MapFrom(src => src.RegistrationDeadline); });
        }
    }
}