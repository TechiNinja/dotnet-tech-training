using SportsManagementApp.Entities;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface IEventRequestRepository : IGenericRepository<EventRequest>
    {
        Task<EventRequest?> GetByIdWithDetailsAsync(int id);
    }
}