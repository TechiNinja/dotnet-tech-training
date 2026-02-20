using SportsManagementApp.Data.DTOs.RoleManagement;
using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Services.Interfaces
{
    public interface IRoleService
    {
        Task<List<Role>> GetRolesAsync();
        Task<Role> CreateRoleAsync(CreateRoleDto createRole);
    }
}
