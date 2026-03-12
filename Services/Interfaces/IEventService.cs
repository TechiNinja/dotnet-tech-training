using SportsManagementApp.DTOs.Request;
using SportsManagementApp.DTOs.Response;

namespace SportsManagementApp.Services.Interfaces
{
    public interface IEventService
    {
        Task<IEnumerable<EventResponse>> GetAllAsync(EventFilterRequest filter);
        Task<EventResponse> GetByIdAsync(int eventId);
        Task<EventResponse> CreateEventFromRequestAsync(CreateEventRequest request);
        Task<IEnumerable<EventCategoryResponse>> GetCategoriesByEventIdAsync(int eventId);
        Task<EventRequestPreFillResponse> GetEventRequestForPreFillAsync(int requestId);
        Task<EventResponse> AssignOrganizerAsync(int eventId, int organizerId);
    }
}
