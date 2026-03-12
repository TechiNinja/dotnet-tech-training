using SportsManagementApp.Data.DTOs.UserManagement;
using SportsManagementApp.Data.Entities;
using System.Linq.Expressions;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<List<User>> GetUsersWithRoleAsync();
        Task<User?> GetUserEntityByIdAsync(int userId);
        Task<UserResponseDto?> GetUserDtoByIdAsync(int userId, Expression<Func<User, UserResponseDto>> projection);
        Task<List<UserResponseDto>> GetUsersByFilterAsync(Expression<Func<User, bool>> predicate, Expression<Func<User, UserResponseDto>> projection);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
    }
}