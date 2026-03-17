using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Data.Filters;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface IEventRequestRepository : IGenericRepository<EventRequest>
    {
        Task<EventRequest?> GetEventRequestByIdAsync(int id);
        Task<EventRequestResponseDto?> GetEventRequestDtoByIdAsync(int id);
        Task<List<EventRequestResponseDto>> GetEventRequestsByFilterAsync(EventRequestFilterDto filter);
    }
}