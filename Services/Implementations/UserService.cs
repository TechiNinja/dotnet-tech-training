using AutoMapper;
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
        private readonly IMapper _mapper;
        private readonly PasswordHasher<User> _passwordHasher = new();

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<List<LoginResponseDto>> GetUsersAsync()
        {
            var users = await _userRepository.GetUsersWithRoleAsync();
            return _mapper.Map<List<LoginResponseDto>>(users);
        }

        public async Task<LoginResponseDto?> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetUserEntityByIdAsync(userId);
            return _mapper.Map<LoginResponseDto?>(user);
        }

        public async Task<User> CreateUserAsync(CreateUserDto createUser)
        {
            var user = _mapper.Map<User>(createUser);
            user.CreatedAt = DateTime.UtcNow;
            user.IsActive = true;
            user.PasswordHash = _passwordHasher.HashPassword(user, createUser.Password);

            await _userRepository.AddAsync(user);
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

            await _userRepository.UpdateAsync(user);
            return _mapper.Map<UserResponseDto>(user);
        }
    }
}