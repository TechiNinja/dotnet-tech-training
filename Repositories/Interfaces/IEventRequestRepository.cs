using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Data.Filters;
using SportsManagementApp.Enums;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface IEventRequestRepository : IGenericRepository<EventRequest>
    {
        Task<EventRequest?> GetByIdWithDetailsAsync(int id);
        Task<EventRequest?> GetEventRequestById(int id);
        Task<IEnumerable<EventRequest>> Search(int? id, RequestStatus? status);
        Task<EventRequest?> GetEventRequestByIdAsync(int id);
        Task<EventRequestResponseDto?> GetEventRequestDtoByIdAsync(int id);
        Task<List<EventRequestResponseDto>> GetEventRequestsByFilterAsync(EventRequestFilterDto filter);
    }
}