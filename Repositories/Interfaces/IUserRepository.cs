using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface IUserRepository: IGenericRepository<User>
    {
        Task<List<User>> GetUsersWithRoleAsync();
    }
}
