using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface IOperationsRepository : IGenericRepository<EventRequest>
    {
        Task<EventRequest?> GetEventRequestByIdAsync(int id);
    }
}