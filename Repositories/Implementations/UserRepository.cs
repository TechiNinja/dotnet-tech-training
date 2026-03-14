﻿using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.Data.DTOs.UserManagement;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Repositories.Interfaces;
using System.Linq.Expressions;

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

        public async Task<UserResponseDto?> GetUserDtoByIdAsync(int userId, Expression<Func<User, UserResponseDto>> projection)
        {
            return await _dbSet
                .Where(user => user.Id == userId)
                .Select(projection)
                .FirstOrDefaultAsync();
        }

        public async Task<List<UserResponseDto>> GetUsersByFilterAsync(Expression<Func<User, bool>> predicate, Expression<Func<User, UserResponseDto>> projection)
        {
            return await GetAllAsync(predicate, projection);
        }
    }
}