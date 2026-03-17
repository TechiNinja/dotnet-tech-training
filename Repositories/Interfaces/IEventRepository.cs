using SportsManagementApp.Data.Entities;
using SportsManagementApp.Data.Filters;
using SportsManagementApp.DTOs.EventCreation;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface IEventRepository : IGenericRepository<Event>
    {
        Task<Event?> GetByIdWithDetailsAsync(int eventId);
        Task<IEnumerable<EventResponseDto>> GetProjectedListAsync(EventFilterDto filter);
        Task<bool> ExistsByRequestIdAsync(int requestId);
    }
}