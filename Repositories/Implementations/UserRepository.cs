using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Repositories.Interfaces;

namespace SportsManagementApp.Repositories.Implementations
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<User?> GetByIdWithRoleAsync(int userId) =>
            await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);

        public async Task<List<User>> GetUsersWithRoleAsync() =>
            await _dbSet.Include(u => u.Role).ToListAsync();

        public async Task<User?> GetUserEntityByIdAsync(int userId) =>
            await _dbSet.Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);
    }
}