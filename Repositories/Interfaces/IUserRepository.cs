using SportsManagementApp.Data.Entities;
using System.Linq.Expressions;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface IUserRepository: IGenericRepository<User>
    {
        Task<List<User>> GetUsersWithRoleAsync();
    }
}
