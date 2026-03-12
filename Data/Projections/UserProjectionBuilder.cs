using SportsManagementApp.Data.DTOs.UserManagement;
using SportsManagementApp.Data.Entities;
using System.Linq.Expressions;

namespace SportsManagementApp.Data.Projections
{
    public static class UserProjectionBuilder
    {
        public static Expression<Func<User, UserResponseDto>> Build()
        {
            return user => new UserResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                RoleId = user.RoleId,
                RoleName = user.Role!.Name,
                IsActive = user.IsActive
            };
        }
    }
}
