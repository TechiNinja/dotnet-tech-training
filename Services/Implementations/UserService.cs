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

        public async Task<List<UserResponseDto>> GetUsersAsync()
        {
            var users = await _userRepository.GetUsersWithRoleAsync();
            return _mapper.Map<List<UserResponseDto>>(users);
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetUserEntityByIdAsync(userId);
            return _mapper.Map<UserResponseDto?>(user);
        }

        public async Task<UserResponseDto> CreateUserAsync(CreateUserDto createUser)
        {
            var user = _mapper.Map<User>(createUser);
            user.CreatedAt = DateTime.UtcNow;
            user.IsActive = true;
            user.PasswordHash = _passwordHasher.HashPassword(user, createUser.Password);

            await _userRepository.AddAsync(user);
            return _mapper.Map<UserResponseDto>(user);
        }

        public async Task<UserResponseDto?> UpdateUserAsync(int userId, UpdateUserDto updateUser)
        {
            var user = await _userRepository.GetUserEntityByIdAsync(userId);
            if (user == null) return null;

            _mapper.Map(updateUser, user);

            if (!string.IsNullOrWhiteSpace(updateUser.Password))
                user.PasswordHash = _passwordHasher.HashPassword(user, updateUser.Password);

            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            return _mapper.Map<UserResponseDto>(user);
        }
    }
}