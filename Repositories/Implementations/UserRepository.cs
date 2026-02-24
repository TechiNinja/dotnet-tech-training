using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Repositories.Interfaces;

namespace SportsManagementApp.Repositories.Implementations
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<List<User>> GetUsersWithRoleAsync()
        {
            return await _dbSet.Include(user => user.Role).ToListAsync();
        }

        public async Task<User?> GetUserEntityByIdAsync(int userId)
        {
            return await _dbSet.Include(user => user.Role)
                               .FirstOrDefaultAsync(user => user.Id == userId);
        }
    }
}