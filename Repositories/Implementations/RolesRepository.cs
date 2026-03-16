﻿using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Repositories.Interfaces;

namespace SportsManagementApp.Repositories.Implementations
{
    public class RolesRepository : GenericRepository<Role>, IRoleRepository
    {
        public RolesRepository(AppDbContext context) : base(context) { }

        public async Task<List<Role>> GetRolesAsync()
        {
            var roles = await GetAllListAsync();
            return roles.ToList();
        }

        public async Task<Role?> GetRoleByTypeAsync(string roleName) =>
            await _dbSet.FirstOrDefaultAsync(role => role.Name == roleName);

        public async Task AddRoleAsync(Role role) =>
            await AddAsync(role);
    }
}
