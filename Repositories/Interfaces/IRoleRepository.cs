using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<List<Role>> GetRolesAsync();
        Task<Role?> GetRoleByTypeAsync(string RoleName);
        Task AddRoleAsync(Role role);
    }
}
