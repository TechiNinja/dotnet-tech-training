using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Repositories.Interfaces;

namespace SportsManagementApp.Repositories.Implementations
{
    public class AuthRepository: GenericRepository<User>, IAuthRepository
    {
        public AuthRepository(AppDbContext context): base(context) { }

        public async Task<User?> GetUserByEmailWithRoleAsync(string email)
        {
            return await _context.Users
                .Include(user => user.Role)
                .FirstOrDefaultAsync(user => user.Email == email);
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(user => user.Email == email);
        }
    }
}
