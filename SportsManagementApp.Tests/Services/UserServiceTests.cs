using AutoMapper;
using Moq;
using SportsManagementApp.Data.DTOs.UserManagement;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Implementations;
using SportsManagementApp.Tests.TestData;
using System.Linq.Expressions;

namespace SportsManagementApp.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _mockRepo = new Mock<IUserRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new UserService(_mockRepo.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task GetUsersAsync_ReturnsMappedUserList()
        {
            var users = UserTestData.UserEntityList();
            var expected = UserTestData.UserResponseList();

            _mockRepo.Setup(repo => repo.GetUsersWithRoleAsync()).ReturnsAsync(users);
            _mockMapper.Setup(mapper => mapper.Map<List<UserResponseDto>>(users)).Returns(expected);

            var result = await _service.GetUsersAsync();

            Assert.Equal(expected.Count, result.Count);
            Assert.Equal(expected[0].FullName, result[0].FullName);
        }

        [Fact]
        public async Task GetUserByIdAsync_WhenUserExists_ReturnsMappedDto()
        {
            var user = UserTestData.UserHimani();
            var expected = UserTestData.UserHimaniResponse();

            _mockRepo.Setup(repo => repo.GetByIdAsync(4)).ReturnsAsync(user);
            _mockMapper.Setup(mapper => mapper.Map<UserResponseDto>(user)).Returns(expected);

            var result = await _service.GetUserByIdAsync(4);

            Assert.NotNull(result);
            Assert.Equal(expected.FullName, result!.FullName);
        }

        [Fact]
        public async Task GetUserByIdAsync_WhenUserNotFound_ReturnsNull()
        {
            _mockRepo.Setup(repo => repo.GetByIdAsync(99)).ReturnsAsync((User?)null);

            var result = await _service.GetUserByIdAsync(99);

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateUserAsync_WhenEmailAlreadyExists_ThrowsConflictException()
        {
            var dto = UserTestData.DuplicateEmailDto();

            var existingUsers = new List<UserResponseDto>
            {
                new() { Id = 2, Email = "vishal@test.com" }
            };

            _mockRepo.Setup(repo => repo.GetAllAsync(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<Expression<Func<User, UserResponseDto>>>()))
                .ReturnsAsync(existingUsers);

            var exception = await Assert.ThrowsAsync<ConflictException>(
                async () => await _service.CreateUserAsync(dto));
            Assert.Equal("User with this email already exists", exception.Message);
        }

        [Fact]
        public async Task CreateUserAsync_WhenValidInput_CreatesAndReturnsUser()
        {
            var dto = UserTestData.ValidCreateDto();

            var mappedUser = UserTestData.MappedNewUser();
            var savedDto = UserTestData.CreatedHimeshResponse();

            _mockRepo.Setup(repo => repo.GetAllAsync(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<Expression<Func<User, UserResponseDto>>>()))
                .ReturnsAsync(new List<UserResponseDto>());

            _mockMapper.Setup(mapper => mapper.Map<User>(dto)).Returns(mappedUser);
            _mockRepo.Setup(repo => repo.AddAsync(mappedUser)).Returns(Task.CompletedTask);
            _mockRepo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);
            _mockRepo.Setup(repo => repo.GetByIdAsync(mappedUser.Id)).ReturnsAsync(mappedUser);
            _mockMapper.Setup(mapper => mapper.Map<UserResponseDto>(mappedUser)).Returns(savedDto);

            var result = await _service.CreateUserAsync(dto);

            Assert.NotNull(result);
            Assert.Equal("himesh@test.com", result.Email);
            _mockRepo.Verify(repo => repo.AddAsync(mappedUser), Times.Once);
            _mockRepo.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_WhenUserNotFound_ThrowsNotFoundException()
        {
            _mockRepo.Setup(repo => repo.GetByIdAsync(99)).ReturnsAsync((User?)null);

            var exception = await Assert.ThrowsAsync<NotFoundException>(
                async () => await _service.UpdateUserAsync(99, new UpdateUserDto()));
            Assert.Equal("User not found", exception.Message);
        }

        [Fact]
        public async Task UpdateUserAsync_WhenPasswordProvided_HashesPassword()
        {
            var user = UserTestData.UserVishalWithOldHash();
            var dto = UserTestData.UpdateVishalWithPassword();
            var updatedDto = UserTestData.UpdatedVishalResponse();

            _mockRepo.Setup(repo => repo.GetByIdAsync(2)).ReturnsAsync(user);
            _mockMapper.Setup(mapper => mapper.Map(dto, user));
            _mockRepo.Setup(repo => repo.UpdateAsync(user)).Returns(Task.CompletedTask);
            _mockRepo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);
            _mockRepo.Setup(repo => repo.GetAllAsync(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<Expression<Func<User, UserResponseDto>>>()))
                .ReturnsAsync(new List<UserResponseDto> { updatedDto });

            var result = await _service.UpdateUserAsync(2, dto);

            Assert.NotNull(result);
            Assert.NotEqual("oldhash", user.PasswordHash);
            _mockRepo.Verify(repo => repo.UpdateAsync(user), Times.Once);
            _mockRepo.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_WhenPasswordNotProvided_DoesNotChangePasswordHash()
        {
            var user = UserTestData.UserNavneetWithOldHash();
            var dto = UserTestData.UpdateNavneetWithoutPassword();
            var updatedDto = UserTestData.UpdatedNavneetResponse();

            _mockRepo.Setup(repo => repo.GetByIdAsync(5)).ReturnsAsync(user);
            _mockMapper.Setup(mapper => mapper.Map(dto, user));
            _mockRepo.Setup(repo => repo.UpdateAsync(user)).Returns(Task.CompletedTask);
            _mockRepo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);
            _mockRepo.Setup(repo => repo.GetAllAsync(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<Expression<Func<User, UserResponseDto>>>()))
                .ReturnsAsync(new List<UserResponseDto> { updatedDto });

            var result = await _service.UpdateUserAsync(5, dto);

            Assert.NotNull(result);
            Assert.Equal("oldhash", user.PasswordHash);
            _mockRepo.Verify(repo => repo.UpdateAsync(user), Times.Once);
            _mockRepo.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_WhenValidInput_ReturnsUpdatedDto()
        {
            var user = UserTestData.UserVaishak();
            var dto = UserTestData.UpdateVaishak();
            var updatedDto = UserTestData.UpdatedVaishakResponse();

            _mockRepo.Setup(repo => repo.GetByIdAsync(6)).ReturnsAsync(user);
            _mockMapper.Setup(mapper => mapper.Map(dto, user));
            _mockRepo.Setup(repo => repo.UpdateAsync(user)).Returns(Task.CompletedTask);
            _mockRepo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);
            _mockRepo.Setup(repo => repo.GetAllAsync(
                It.IsAny<Expression<Func<User, bool>>>(),
                It.IsAny<Expression<Func<User, UserResponseDto>>>()))
                .ReturnsAsync(new List<UserResponseDto> { updatedDto });

            var result = await _service.UpdateUserAsync(6, dto);

            Assert.Equal("Vaishak S", result!.FullName);
            Assert.False(result.IsActive);
            _mockRepo.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }
    }
}
