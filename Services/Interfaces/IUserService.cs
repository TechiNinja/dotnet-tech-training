using SportsManagementApp.Data.DTOs.Auth;
using SportsManagementApp.Data.DTOs.UserManagement;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Data.Filters;

namespace SportsManagementApp.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserResponseDto>> GetUsersAsync();
        Task<List<UserResponseDto>> GetUsersByFilterAsync(UserFilterDto filter);
        Task<UserResponseDto?> GetUserByIdAsync(int userId);
        Task<UserResponseDto> CreateUserAsync(CreateUserDto createUser);
        Task<UserResponseDto?> UpdateUserAsync(int userId, UpdateUserDto updateUser);
    }
}
