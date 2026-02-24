using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SportsManagementApp.Constants;
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
        private readonly IRoleRepository _roleRepository;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<User> _passwordHasher;
        private readonly IMapper _mapper;

        public AuthService(IAuthRepository authRepository, IConfiguration configuration, IRoleRepository roleRepository, IMapper mapper)
        {
            _authRepository = authRepository;
            _roleRepository = roleRepository;
            _configuration = configuration;
            _passwordHasher = new PasswordHasher<User>();
            _mapper = mapper;
        }

        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto loginRequest)
        {
            var user = await _authRepository.GetUserByEmailWithRoleAsync(loginRequest.Email);

            if (user == null)
            {
                throw new Exception("Invalid email or password");
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginRequest.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                throw new Exception("Invalid password");
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

            var response = _mapper.Map<LoginResponseDto>(user);
            response.Token = token;

            return response;
        }

        public async Task<LoginResponseDto> RegisterAsync(RegisterRequestDto registerRequest)
        {
            if (await _authRepository.UserExistsAsync(registerRequest.Email))
            {
                throw new Exception("User already exists");
            }

            var participantRole = await _roleRepository.GetRoleByTypeAsync(RoleConstants.Participant);

            if (participantRole == null)
            {
                throw new Exception("Participant Role not found");
            }

            var user = _mapper.Map<User>(registerRequest);
            user.RoleId = participantRole.Id;
            user.IsActive = true;
            user.CreatedAt = DateTime.UtcNow;

            user.PasswordHash = _passwordHasher.HashPassword(user, registerRequest.Password);

            await _authRepository.AddUserAsync(user);

            var token = GenerateJwtToken(user);

            var response = _mapper.Map<LoginResponseDto>(user);
            response.Token = token;

            return response;
        }

        private string GenerateJwtToken(User user)
        {
            var key = Encoding.UTF8.GetBytes(_configuration[JwtConstant.Key] ?? throw new Exception("JWT Key missing"));

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role!.Name)
            };

            var creds = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration[JwtConstant.Issuer],
                audience: _configuration[JwtConstant.Audience],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}