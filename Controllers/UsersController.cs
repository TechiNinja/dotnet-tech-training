using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportsManagementApp.Constants;
using SportsManagementApp.Data.DTOs.UserManagement;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize(Roles = RoleConstants.Admin)]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _userService.GetUsersAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
            {
                return NotFound("User not found");
            }

            return Ok(user);
        }

        [Authorize(Roles = RoleConstants.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserDto createUser)
        {
            var user = await _userService.CreateUserAsync(createUser);

            return Ok(user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UpdateUserDto updateUser)
        {
            var result = await _userService.UpdateUserAsync(id, updateUser);
            
            if (result == null)
            {
                return NotFound("User not found");
            }

            return Ok(result);
        }
    }
}
