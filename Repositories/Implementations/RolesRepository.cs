using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Repositories.Interfaces;

namespace SportsManagementApp.Repositories.Implementations
{
    public class RolesRepository: IRoleRepository
    {
        private readonly AppDbContext _context;

        public RolesRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Role>> GetRolesAsync()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<Role?> GetRoleByTypeAsync(string roleName)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(role => role.Name == roleName);
        }

        public async Task AddRoleAsync(Role role)
        {
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
        }
    }
}
