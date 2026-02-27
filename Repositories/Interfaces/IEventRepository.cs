using SportsManagementApp.Entities;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface IEventRepository : IGenericRepository<Event>
    {
        Task<Event?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Event>> GetAllWithCategoriesAsync(string? status, string? name, int? sportId);
        Task<bool> ExistsByRequestIdAsync(int requestId);
    }
}