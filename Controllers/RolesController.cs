using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportsManagementApp.Data.DTOs.RoleManagement;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _roleService.GetRolesAsync();
            return Ok(roles.Select(role => new
            {
                role.Id,
                role.Name,
            }));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto createRole)
        {
            try
            {
                var role = await _roleService.CreateRoleAsync(createRole);
                return Ok(role);
            }
            catch (Exception)
            {
                return Problem("An error occurred while creating role");
            }
        }
    }
}
