using SportsManagementApp.Data.DTOs.Auth;

namespace SportsManagementApp.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest);
        Task<LoginResponseDto> RegisterAsync(RegisterRequestDto registerRequest);
    }
}
