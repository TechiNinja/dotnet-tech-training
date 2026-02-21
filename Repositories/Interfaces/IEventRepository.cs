using SportsManagementApp.Entities;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface IEventRepository
    {
        Task<Event?> GetByIdAsync(int id, bool includeCategories = false);
        Task<List<Event>> GetAllAsync();
        Task<bool> ExistsByNameSportDateAsync(string name, int sportId, DateOnly startDate);
        Task AddAsync(Event eventEntity);
        void Update(Event eventEntity);
        Task SaveChangesAsync();
    }

    public interface IEventRequestRepository
    {
        Task<EventRequest?> GetByIdAsync(int id);
    }

    public interface IUserRepository
    {
        Task<User?> GetByIdWithRoleAsync(int userId);
    }
}