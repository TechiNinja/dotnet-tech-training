using SportsManagementApp.Data.Entities;
using SportsManagementApp.Data.Filters;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface IEventRequestRepository : IGenericRepository<EventRequest>
    {
        Task<EventRequest?> GetEventRequestByIdAsync(int id);
        Task<List<EventRequest>> GetEventRequestsByFilterAsync(EventRequestFilterDto filter);
    }
}