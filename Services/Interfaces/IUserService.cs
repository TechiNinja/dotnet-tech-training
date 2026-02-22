using SportsManagementApp.Data.DTOs.Auth;
using SportsManagementApp.Data.DTOs.UserManagement;
using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<LoginResponseDto>> GetUsersAsync();
        Task<LoginResponseDto?> GetUserByIdAsync(int userId);
        Task<User> CreateUserAsync(CreateUserDto createUser);
        Task<UserResponseDto?> UpdateUserAsync(int userId, UpdateUserDto updateUser);
    }
}
