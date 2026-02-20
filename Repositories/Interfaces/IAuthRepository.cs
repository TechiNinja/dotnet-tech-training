using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByEmailWithRoleAsync(string email);
        Task<bool> UserExistsAsync(string email);
        Task<Role?> GetParticipantRoleAsync();
        Task AddUserAsync(User user);
    }
}
