using SportsManagementApp.Data.Filters;
using SportsManagementApp.DTOs.EventCreation;

namespace SportsManagementApp.Services.Interfaces
{
    public interface IEventService : IGenericService<EventResponseDto>
    {
        Task<IEnumerable<EventResponseDto>> GetAllAsync(EventFilterDto filter);
        Task<EventResponseDto> CreateEventFromRequestAsync(CreateEventDto request);
        Task<EventRequestPreFillResponseDto> GetEventRequestForPreFillAsync(int requestId);
        Task<EventResponseDto> AssignOrganizerAsync(int eventId, int organizerId);
        Task<EventResponseDto> PatchEventAsync(int eventId, PatchEventRequestDto request);
    }
}