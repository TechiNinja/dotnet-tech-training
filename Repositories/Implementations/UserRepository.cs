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

        public async Task<List<User>> GetUsersWithRoleAsync()
        {
            return await _dbSet
                .Include(user => user.Role)
                .ToListAsync();
        }

        public async Task<User?> GetUserEntityByIdAsync(int userId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(user => user.Id == userId);
        }

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