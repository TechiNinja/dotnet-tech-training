using Microsoft.AspNetCore.Mvc;
using SportsManagementApp.Data.DTOs.Auth;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto loginRequest)
        {
            var result = await _authService.LoginAsync(loginRequest);

            if (result == null)
            {
                return Unauthorized("Invalid email or password");
            }

            return Ok(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto registerRequest)
        {
            var result = await _authService.RegisterAsync(registerRequest);
            return Ok(result);
        }
    }
}
