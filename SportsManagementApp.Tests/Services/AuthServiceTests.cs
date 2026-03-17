using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;
using Moq;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Implementations;
using System;
using System.Collections.Generic;
using System.Text;
using SportsManagementApp.Tests.TestData;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Data.DTOs.Auth;
using SportsManagementApp.Constants;
using FluentAssertions;

namespace SportsManagementApp.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IAuthRepository> _mockAuthRepo;
        private readonly Mock<IRoleRepository> _mockRoleRepo;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<IMapper> _mockMapper;
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            _mockAuthRepo = new Mock<IAuthRepository>();
            _mockRoleRepo = new Mock<IRoleRepository>();
            _mockConfig = new Mock<IConfiguration>();
            _mockMapper = new Mock<IMapper>();

            _mockConfig.Setup(config => config["Jwt:Key"]).Returns("supersecretkey1234567890abcdefgh");
            _mockConfig.Setup(config => config["Jwt:Issuer"]).Returns("TestIssuer");
            _mockConfig.Setup(config => config["Jwt:Audience"]).Returns("TestAudience");

            _service = new AuthService(
                _mockAuthRepo.Object,
                _mockConfig.Object,
                _mockRoleRepo.Object,
                _mockMapper.Object
            );
        }

        [Fact]
        public async Task LoginAsync_WhenUserNotFound_ThrowsUnauthorizedException()
        {
            var request = AuthTestData.NonExistentEmailRequest();
            _mockAuthRepo.Setup(repo => repo.GetUserByEmailWithRoleAsync(request.Email))
                .ReturnsAsync((User?)null);

            var exception = await Assert.ThrowsAsync<UnauthorizedException>(
                async () => await _service.LoginAsync(request));
            Assert.Equal("Invalid email or password", exception.Message);
        }

        [Fact]
        public async Task LoginAsync_WhenPasswordIncorrect_ThrowsUnauthorizedException()
        {
            var request = AuthTestData.WrongPasswordRequest();
            var user = AuthTestData.UserWithMismatchedPassword();
            _mockAuthRepo.Setup(repo => repo.GetUserByEmailWithRoleAsync(request.Email))
                .ReturnsAsync(user);

            var exception = await Assert.ThrowsAsync<UnauthorizedException>(
                async () => await _service.LoginAsync(request));
            Assert.Equal("Invalid password", exception.Message);
        }

        [Fact]
        public async Task LoginAsync_WhenUserIsDeactivated_ThrowsBadRequestException()
        {
            var request = AuthTestData.DeactivatedUserLoginRequest();
            var user = AuthTestData.DeactivatedUser();
            _mockAuthRepo.Setup(repo => repo.GetUserByEmailWithRoleAsync(request.Email))
                .ReturnsAsync(user);

            var exception = await Assert.ThrowsAsync<BadRequestException>(
                async () => await _service.LoginAsync(request));
            Assert.Equal("User is deactivated", exception.Message);
        }

        [Fact]
        public async Task LoginAsync_WhenRoleIsNull_ThrowsBadRequestException()
        {
            var request = AuthTestData.NullRoleUserLoginRequest();
            var user = AuthTestData.UserWithNullRole();
            _mockAuthRepo.Setup(repo => repo.GetUserByEmailWithRoleAsync(request.Email))
                .ReturnsAsync(user);

            var exception = await Assert.ThrowsAsync<BadRequestException>(
                async () => await _service.LoginAsync(request));
            Assert.Equal("User role is not assigned", exception.Message);
        }

        [Fact]
        public async Task LoginAsync_WhenValidCredentials_ReturnsLoginResponseWithToken()
        {
            var request = AuthTestData.ValidLoginRequest();
            var user = AuthTestData.ActiveAdminUser();
            var responseDto = AuthTestData.LoginResponseForHimani();
            _mockAuthRepo.Setup(repo => repo.GetUserByEmailWithRoleAsync(request.Email)).ReturnsAsync(user);
            _mockMapper.Setup(mapper => mapper.Map<LoginResponseDto>(user)).Returns(responseDto);

            var result = await _service.LoginAsync(request);

            Assert.NotNull(result);
            Assert.Equal("himani.jangid@intimetec.com", result.Email);
            Assert.False(string.IsNullOrEmpty(result.Token));
            _mockAuthRepo.Verify(repo => repo.GetUserByEmailWithRoleAsync(request.Email), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_WhenEmailAlreadyExists_ThrowsConflictException()
        {
            var request = AuthTestData.ExistingEmailRegisterRequest();
            _mockAuthRepo.Setup(repo => repo.UserExistsAsync(request.Email)).ReturnsAsync(true);

            var exception = await Assert.ThrowsAsync<ConflictException>(
                async () => await _service.RegisterAsync(request));
            Assert.Equal("User already exists", exception.Message);
        }

        [Fact]
        public async Task RegisterAsync_WhenParticipantRoleNotFound_ThrowsNotFoundException()
        {
            var request = AuthTestData.NewUserRegisterRequest();
            _mockAuthRepo.Setup(repo => repo.UserExistsAsync(request.Email)).ReturnsAsync(false);
            _mockRoleRepo.Setup(repo => repo.GetRoleByTypeAsync(RoleConstants.Participant))
                .ReturnsAsync((Role?)null);

            var exception = await Assert.ThrowsAsync<NotFoundException>(
                async () => await _service.RegisterAsync(request));
            Assert.Equal("Participant Role not found", exception.Message);
        }

        [Fact]
        public async Task RegisterAsync_WhenValidInput_CreatesUserAndReturnsTokenResponse()
        {
            var request = AuthTestData.ValidRgisterRequest();
            var participantRole = AuthTestData.ParticipantRole();
            var mappedUser = AuthTestData.MappedUserForRegister(participantRole);
            var responseDto = AuthTestData.LoginResponseForPiyush();

            _mockAuthRepo.Setup(repo => repo.UserExistsAsync(request.Email)).ReturnsAsync(false);
            _mockRoleRepo.Setup(repo => repo.GetRoleByTypeAsync(RoleConstants.Participant))
                .ReturnsAsync(participantRole);
            _mockMapper.Setup(mapper => mapper.Map<User>(request)).Returns(mappedUser);
            _mockAuthRepo.Setup(repo => repo.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
            _mockAuthRepo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);
            _mockMapper.Setup(mapper => mapper.Map<LoginResponseDto>((It.IsAny<User>()))).Returns(responseDto);

            var result = await _service.RegisterAsync(request);

            Assert.NotNull(result);
            Assert.Equal("piyush@test.com", result.Email);
            Assert.False(string.IsNullOrEmpty(result.Token));
            _mockAuthRepo.Verify(repo => repo.AddAsync(It.IsAny<User>()), Times.Once);
            _mockAuthRepo.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }
    }
}
