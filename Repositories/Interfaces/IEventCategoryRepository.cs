using SportsManagementApp.Data.Entities;
using SportsManagementApp.Enums;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface IEventCategoryRepository : IGenericRepository<EventCategory>
    {
        Task<EventCategory?> GetByIdWithDetailsAsync(int eventCategoryId);
        Task UpdateEventStatusAsync(int eventCategoryId, EventStatus status);
    }
}