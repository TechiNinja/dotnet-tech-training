using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SportsManagementApp.Data.DTOs.Auth;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SportsManagementApp.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<User> _passwordHasher;

        public AuthService(IAuthRepository authRepository, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest)
        {
            var user = await _authRepository.GetUserByEmailWithRoleAsync(loginRequest.Email);

            if (user == null)
            {
                return null;
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginRequest.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                return null;
            }

            if (!user.IsActive)
            {
                throw new Exception("User is deactivated");
            }

            if (user.Role == null)
            {
                throw new Exception("User role is not assigned");
            }

            var token = GenerateJwtToken(user);

            return new LoginResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role.Name,
                Token = token
            };
        }

        public async Task<LoginResponseDto> RegisterAsync(RegisterRequestDto registerRequest)
        {
            if (await _authRepository.UserExistsAsync(registerRequest.Email))
            {
                throw new Exception("User already exists");
            }

            var participantRole = await _authRepository.GetParticipantRoleAsync();

            if (participantRole == null)
            {
                throw new Exception("Participant Role not found");
            }

            var user = new User
            {
                FullName = registerRequest.FullName,
                Email = registerRequest.Email,
                RoleId = participantRole.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, registerRequest.Password);

            await _authRepository.AddUserAsync(user);
            user.Role = participantRole;

            var token = GenerateJwtToken(user);

            return new LoginResponseDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = "Participant",
                Token = token
            };
        }

        private string GenerateJwtToken(User user)
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new Exception("JWT Key missing"));

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role?.Name ?? "Participant")
            };

            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}