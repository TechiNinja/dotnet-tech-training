using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportsManagementApp.Constants;
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
            return Ok(await _roleService.GetRolesAsync());
        }

        [Authorize(Roles = RoleConstants.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto createRole)
        {
            var role = await _roleService.CreateRoleAsync(createRole);
            return Ok(role);
        }
    }
}
