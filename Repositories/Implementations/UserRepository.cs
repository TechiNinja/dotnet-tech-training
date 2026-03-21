using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Repositories.Interfaces;
using System.Linq.Expressions;

namespace SportsManagementApp.Repositories.Implementations
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<List<User>> GetUsersWithRoleAsync()
        {
            return await _dbSet
                .Include(user => user.Role)
                .ToListAsync();
        }

        public async Task<List<TResult>> GetUsersAsync<TResult>(
            Expression<Func<User, TResult>> projection)
        {
            return await _dbSet
                .Select(projection)
                .ToListAsync();
        }

        public async Task<List<TResult>> GetUsersAsyncWithFilter<TResult>(
            Expression<Func<User, bool>> predicate,
            Expression<Func<User, TResult>> projection)
        {
            return await _dbSet
                .Where(predicate)
                .Select(projection)
                .ToListAsync();
        }

        public async Task<User?> GetUserEntityByIdAsync(int id)
        {
            return await _dbSet
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}
