using SportsManagementApp.Data.DTOs.Auth;
using SportsManagementApp.Data.DTOs.UserManagement;
using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<List<LoginResponseDto>> GetUsersAsync();
        Task<LoginResponseDto?> GetUserByIdAsync(int id);
        Task<User> CreateUserAsync(User user);   
        Task UpdateUserAsync(User user);
        Task<User?> GetUserByIdEntityAsync(int userId);
        Task AddUserAsync(User user);            
    }
}