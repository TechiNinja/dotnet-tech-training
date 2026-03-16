using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface IAuthRepository: IGenericRepository<User>
    {
        Task<User?> GetUserByEmailWithRoleAsync(string email);
        Task<bool> UserExistsAsync(string email);
    }
}
