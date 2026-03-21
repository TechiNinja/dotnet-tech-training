using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface IEventCategoryRepository : IGenericRepository<EventCategory>
    {
        Task<EventCategory?> GetByIdWithDetailsAsync(int catId);
    }
}