using Microsoft.AspNetCore.Identity;
using SportsManagementApp.Data.DTOs.Auth;
using SportsManagementApp.Data.DTOs.UserManagement;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordHasher<User> _passwordHasher;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _passwordHasher = new PasswordHasher<User>();
        }

        public Task<List<LoginResponseDto>> GetUsersAsync()
        {
            return _userRepository.GetUsersAsync();
        }

        public Task<LoginResponseDto?> GetUserByIdAsync(int userId)
        {
            return _userRepository.GetUserByIdAsync(userId);
        }

        public async Task<User> CreateUserAsync(CreateUserDto createUser)
        {
            var user = new User
            {
                FullName = createUser.FullName,
                Email = createUser.Email,
                RoleId = createUser.RoleId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, createUser.Password);

            await _userRepository.AddUserAsync(user);
            return user;
        }

        public async Task<UserResponseDto?> UpdateUserAsync(int userId, UpdateUserDto updateUser)
        {
            var user = await _userRepository.GetUserEntityByIdAsync(userId);
            if (user == null) return null;

            if (!string.IsNullOrWhiteSpace(updateUser.FullName))
                user.FullName = updateUser.FullName;

            if (!string.IsNullOrWhiteSpace(updateUser.Email))
                user.Email = updateUser.Email;

            if (!string.IsNullOrWhiteSpace(updateUser.Password))
                user.PasswordHash = _passwordHasher.HashPassword(user, updateUser.Password);

            if (updateUser.RoleId.HasValue)
                user.RoleId = updateUser.RoleId.Value;

            if (updateUser.IsActive.HasValue)
                user.IsActive = updateUser.IsActive.Value;

            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateUserAsync(user);
            return new UserResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                RoleId = user.RoleId,
                RoleName = user.Role?.Name ?? string.Empty,
                IsActive = user.IsActive
            };
        }
    }
}