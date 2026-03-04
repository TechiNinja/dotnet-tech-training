using SportsManagementApp.Data.DTOs.Auth;
using SportsManagementApp.Data.DTOs.UserManagement;
using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserResponseDto>> GetUsersAsync();
        Task<UserResponseDto?> GetUserByIdAsync(int userId);
        Task<UserResponseDto> CreateUserAsync(CreateUserDto createUser);
        Task<UserResponseDto?> UpdateUserAsync(int userId, UpdateUserDto updateUser);
    }
}
