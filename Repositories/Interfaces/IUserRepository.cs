using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User?> GetByIdWithRoleAsync(int userId);
        Task<List<User>> GetUsersWithRoleAsync();
        Task<User?> GetUserEntityByIdAsync(int userId);
    }
}