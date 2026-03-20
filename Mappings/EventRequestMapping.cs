using AutoMapper;
using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Enums;

public class EventRequestMapping : Profile
{
    public EventRequestMapping()
    {
        CreateMap<EventRequest, EventRequestResponseDto>()
    .ForMember(dest => dest.SportsName,
               opt => opt.MapFrom(src => src.Sport != null ? src.Sport.Name : "Unknown"))
    .ForMember(dest => dest.OperationsReviewerName,
               opt => opt.MapFrom(src => src.OperationsReviewer != null ? src.OperationsReviewer.FullName : "Not assigned"))
    .ForMember(dest => dest.AdminName,
               opt => opt.MapFrom(src => src.Admin != null ? src.Admin.FullName : "Unknown"));

        CreateMap<CreateEventRequestDto, EventRequest>()
            .ForMember(dest => dest.CreatedDate,
                opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.Status,
                opt => opt.MapFrom(_ => RequestStatus.Pending));

        CreateMap<BaseEventRequestDto, EventRequest>();
        }
}