using SportsManagementApp.Data.DTOs.RoleManagement;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Services.Implementations
{
    public class RolesService: IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RolesService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<List<Role>> GetRolesAsync()
        {
            return await _roleRepository.GetRolesAsync();
        }

        public async Task<Role> CreateRoleAsync(CreateRoleDto createRole)
        {
            var existing = await _roleRepository.GetRoleByTypeAsync(createRole.RoleName);

            if (existing != null)
            {
                throw new ConflictException("Role already exists");
            }

            var role = new Role
            {
                Name = createRole.RoleName,
            };

            await _roleRepository.AddRoleAsync(role);
            return role;
        }
    }
}
