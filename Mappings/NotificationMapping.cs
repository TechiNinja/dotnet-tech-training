using AutoMapper;
using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Mappings;

public class NotificationMapping : Profile
{
    public NotificationMapping()
    {
        CreateMap<BaseNotificationDto, Notification>()
            .ForMember(dest => dest.CreatedAt,
               opt => opt.MapFrom(src => DateTime.UtcNow));

        CreateMap<Notification, NotificationResponseDto>();
            
    }
}