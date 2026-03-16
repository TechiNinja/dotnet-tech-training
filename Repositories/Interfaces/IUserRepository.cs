using SportsManagementApp.Data.DTOs.UserManagement;
using SportsManagementApp.Data.Entities;
using System.Linq.Expressions;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface IUserRepository: IGenericRepository<User>
    {
        Task<List<User>> GetUsersWithRoleAsync();
        Task<UserResponseDto?> GetUserDtoByIdAsync(int userId, Expression<Func<User, UserResponseDto>> projection);
    }
}