using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.Data.DTOs.Auth;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Repositories.Interfaces;

namespace SportsManagementApp.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<LoginResponseDto>> GetUsersAsync()
        {
            return await _context.Users
                .Include(user => user.Role)
                .Select(user => new LoginResponseDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role!.Name
                })
                .ToListAsync();
        }

        public async Task<LoginResponseDto?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Include(user => user.Role)
                .Where(user => user.Id == id)
                .Select(user => new LoginResponseDto
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Role = user.Role!.Name
                })
                .FirstOrDefaultAsync();
        }

        public async Task<User> CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetUserByIdEntityAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        public async Task AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
    }
}