using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetUsersWithRoleAsync();
        Task<User?> GetUserEntityByIdAsync(int userId);

        Task AddAsync(User user);
        Task UpdateAsync(User user);
    }
}