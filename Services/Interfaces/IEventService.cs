using SportsManagementApp.DTOs.Request;
using SportsManagementApp.DTOs.Response;

namespace SportsManagementApp.Services.Interfaces
{
    public interface IEventService
    {
        Task<EventResponse>       CreateEventFromRequestAsync(CreateEventRequest request);
        Task<EventResponse>       AssignOrganizerAsync(int eventId, AssignOrganizerRequest request);
        Task<EventResponse>       ConfigureEventAsync(int eventId, EventConfigurationRequest request);
        Task<EventResponse>       GetByIdAsync(int eventId);
        Task<List<EventResponse>> GetAllAsync();
    }
}