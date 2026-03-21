using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Data.Filters;

namespace SportsManagementApp.Services.Interfaces
{
    public interface IEventRequestService
    {
        Task<EventRequestResponseDto> RaiseEventRequestAsync(CreateEventRequestDto dto, int adminId);
        Task<EventRequestResponseDto> GetByIdForAdminAsync(int id, int adminId);
        Task<IEnumerable<EventRequestResponseDto>> GetAllEventRequestsAsync(EventRequestFilterDto filter);
        Task<EventRequestResponseDto> EditEventRequestAsync(int id, BaseEventRequestDto dto, int adminId);
        Task<EventRequestResponseDto> WithdrawEventRequestAsync(int id, int adminId);
    }
}