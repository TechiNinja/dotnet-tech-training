using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.Entities;
using SportsManagementApp.Repositories.Interfaces;

namespace SportsManagementApp.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public async Task<User?> GetByIdWithRoleAsync(int userId) =>
            await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);
    }
}