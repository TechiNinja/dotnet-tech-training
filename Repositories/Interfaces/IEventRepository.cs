using SportsManagementApp.DTOs.Response;
using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface IEventRepository : IGenericRepository<Event>
    {
        Task<Event?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<EventResponse>> GetProjectedListAsync(string? status, string? name, int? sportId);
        Task<bool> ExistsByRequestIdAsync(int requestId);
    }
}
