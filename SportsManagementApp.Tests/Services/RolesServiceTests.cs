using SportsManagementApp.Data.Entities;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Implementations;
using SportsManagementApp.Tests.TestData;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace SportsManagementApp.Tests.Services
{
    public class RolesServiceTests
    {
        private readonly Mock<IRoleRepository> _mockRepo;
        private readonly RolesService _service;

        public RolesServiceTests()
        {
            _mockRepo = new Mock<IRoleRepository>();
            _service = new RolesService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetRolesAsync_WhenRolesExist_ReturnsRolesList()
        {
            var expected = RolesTestData.AllRolesList();
            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(expected);

            var result = await _service.GetRolesAsync();

            Assert.Equal(expected.Count, result.Count);
            Assert.Equal(expected[0].Name, result[0].Name);
            Assert.Equal(expected[3].Name, result[3].Name);
        }

        [Fact]
        public async Task GetRolesAsync_WhenNoRoles_ReturnsEmptyList()
        {
            var expected = RolesTestData.EmptyRolesList();
            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(expected);

            var result = await _service.GetRolesAsync();

            Assert.Empty(result);
        }

        [Fact]
        public async Task CreateRoleAsync_WhenRoleAlreadyExists_ThrowsConflictException()
        {
            var dto = RolesTestData.DuplicateRoleDto();
            var existingRole = RolesTestData.ExistingAdminRole();
            _mockRepo.Setup(repo => repo.GetRoleByTypeAsync("Admin")).ReturnsAsync(existingRole);

            var exception = await Assert.ThrowsAsync<ConflictException>(
                async () => await _service.CreateRoleAsync(dto));
            Assert.Equal("Role already exists", exception.Message);
        }

        [Fact]
        public async Task CreateRoleAsync_WhenValidRole_CreatesAndReturnsRole()
        {
            var dto = RolesTestData.ValidCreateRoleDto();
            _mockRepo.Setup(repo => repo.GetRoleByTypeAsync("Manager")).ReturnsAsync((Role?)null);
            _mockRepo.Setup(repo => repo.AddAsync(It.IsAny<Role>())).Returns(Task.CompletedTask);
            _mockRepo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _service.CreateRoleAsync(dto);

            Assert.NotNull(result);
            Assert.Equal("Manager", result.Name);
            _mockRepo.Verify(repo => repo.AddAsync(It.IsAny<Role>()), Times.Once);
            _mockRepo.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }
    }
}
