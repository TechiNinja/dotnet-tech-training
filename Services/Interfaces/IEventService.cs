using SportsManagementApp.DTOs.Request;
using SportsManagementApp.DTOs.Response;

namespace SportsManagementApp.Services.Interfaces
{
    public interface IEventService
    {
        Task<ServiceResult<EventResponse>> CreateEventFromRequestAsync(CreateEventRequest request);
        Task<ServiceResult<EventResponse>> AssignOrganizerAsync(int eventId, AssignOrganizerRequest request);
        Task<ServiceResult<EventResponse>> ConfigureEventAsync(int eventId, EventConfigurationRequest request);
        Task<ServiceResult<EventResponse>> GetByIdAsync(int eventId);
        Task<ServiceResult<List<EventResponse>>> GetAllAsync();
    }
}