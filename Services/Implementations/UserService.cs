using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SportsManagementApp.Data.DTOs.Auth;
using SportsManagementApp.Data.DTOs.UserManagement;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Data.Filters;
using SportsManagementApp.Data.Predicates;
using SportsManagementApp.Data.Projections;
using SportsManagementApp.Exceptions;
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

        public async Task<List<UserResponseDto>> GetUsersByFilterAsync(UserFilterDto filter)
        {
            return await _userRepository.GetAllAsync(
                UserPredicateBuilder.Build(filter),
                UserProjectionBuilder.Build()
            );
        }

        public async Task<UserResponseDto?> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user == null ? null : _mapper.Map<UserResponseDto>(user);
        }

        public async Task<UserResponseDto> CreateUserAsync(CreateUserDto createUser)
        {
            var existingUser = await _userRepository.GetAllAsync(
                UserPredicateBuilder.Build(new UserFilterDto { SearchTerm = createUser.Email }),
                UserProjectionBuilder.Build()
            );

            if (existingUser.Any(user => user.Email.Equals(createUser.Email, StringComparison.OrdinalIgnoreCase)))
                throw new ConflictException("User with this email already exists");

            var user = _mapper.Map<User>(createUser);
            user.CreatedAt = DateTime.UtcNow;
            user.IsActive = true;
            user.PasswordHash = _passwordHasher.HashPassword(user, createUser.Password);

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
            var savedUser = await _userRepository.GetByIdAsync(user.Id);
            return _mapper.Map<UserResponseDto>(savedUser!);
        }

        public async Task<UserResponseDto?> UpdateUserAsync(int userId, UpdateUserDto updateUser)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User not found");

            _mapper.Map(updateUser, user);

            if (!string.IsNullOrWhiteSpace(updateUser.Password))
                user.PasswordHash = _passwordHasher.HashPassword(user, updateUser.Password);

            user.UpdatedAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            var updatedUser = await _userRepository.GetByIdAsync(user.Id);
            return _mapper.Map<UserResponseDto>(updatedUser!);
        }
    }
}