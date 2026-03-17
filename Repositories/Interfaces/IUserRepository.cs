using SportsManagementApp.Data.Entities;
using System.Linq.Expressions;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface IUserRepository: IGenericRepository<User>
    {
        Task<List<User>> GetUsersWithRoleAsync();
        Task<List<TResult>> GetUsersAsync<TResult>(Expression<Func<User, TResult>> projection);
        Task<List<TResult>> GetUsersAsyncWithFilter<TResult>(
            Expression<Func<User, bool>> predicate,
            Expression<Func<User, TResult>> projection);
    }
}
