using SportsManagementApp.Tests.TestData;
using FluentAssertions;
using Moq;
using SportsManagementApp.Data.DTOs.SportManagement;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Implementations;
using System.Linq.Expressions;
using Xunit;

namespace SportsManagementApp.Tests.Services
{
    public class SportServiceTests
    {
        private readonly Mock<ISportRepository> _mockRepo;
        private readonly SportService _service;

        public SportServiceTests()
        {
            _mockRepo = new Mock<ISportRepository>();
            _service = new SportService(_mockRepo.Object);
        }

        [Fact]
        public async Task CreateSportAsync_WhenNameIsEmpty_ThrowsBadRequestException()
        {
            var dto = SportTestData.EmptyNameDto();

            var exception = await Assert.ThrowsAsync<BadRequestException>(
                async () => await _service.CreateSportAsync(dto));
            Assert.Equal("Sport Name is required", exception.Message);
        }

        [Fact]
        public async Task CreateSportAsync_WhenNameIsWhitespace_ThrowsBadRequestException()
        {
            var dto = SportTestData.WhitespaceNameDto();

            var exception = await Assert.ThrowsAsync<BadRequestException>(
                async () => await _service.CreateSportAsync(dto));
            Assert.Equal("Sport Name is required", exception.Message);
        }

        [Fact]
        public async Task CreateSportAsync_WhenSportAlreadyExists_ThrowsConflictException()
        {
            var dto = SportTestData.DuplicateDto();
            _mockRepo.Setup(repo => repo.SportExistsAsync("Badminton")).ReturnsAsync(true);

            var exception = await Assert.ThrowsAsync<ConflictException>(
                async () => await _service.CreateSportAsync(dto));
            Assert.Equal("Sport already exists", exception.Message);
        }

        [Fact]
        public async Task CreateSportAsync_WhenValidInput_CreatesSportAndReturnsIt()
        {
            var dto = SportTestData.ValidCreateDto();
            var expectedSport = SportTestData.CreatedSport();

            _mockRepo.Setup(repo => repo.SportExistsAsync("Pool")).ReturnsAsync(false);
            _mockRepo.Setup(repo => repo.CreateSportAsync("Pool")).ReturnsAsync(expectedSport);

            var result = await _service.CreateSportAsync(dto);

            Assert.NotNull(result);
            Assert.Equal(expectedSport.Name, result.Name);
            _mockRepo.Verify(repo => repo.SportExistsAsync("Pool"), Times.Once);
        }

        [Fact]
        public async Task UpdateSportAsync_WhenNameIsEmpty_ThrowsBadRequestException()
        {
            var dto = SportTestData.EmptyUpdateDto();

            var exception = await Assert.ThrowsAsync<BadRequestException>(
                async () => await _service.UpdateSportAsync(1, dto));
            Assert.Equal("Sport name is required", exception.Message);
        }

        [Fact]
        public async Task UpdateSportAsync_WhenSportNotFound_ThrowsNotFoundException()
        {
            var dto = SportTestData.NotFoundUpdateDto();
            _mockRepo.Setup(repo => repo.GetSportByIdAsync(99)).ReturnsAsync((Sport?)null);

            var exception = await Assert.ThrowsAsync<NotFoundException>(
                async () => await _service.UpdateSportAsync(99, dto));
            Assert.Equal("Sport not found", exception.Message);
        }

        [Fact]
        public async Task UpdateSportAsync_WhenNewNameAlreadyTakenByAnotherSport_ThrowsConflictException()
        {
            var existing = SportTestData.ExistingSport();
            var dto = SportTestData.ConflictingUpdateDto();

            _mockRepo.Setup(repo => repo.GetSportByIdAsync(1)).ReturnsAsync(existing);
            _mockRepo.Setup(repo => repo.SportExistsAsync("Carrom")).ReturnsAsync(true);

            var exception = await Assert.ThrowsAsync<ConflictException>(
                async () => await _service.UpdateSportAsync(1, dto));
            Assert.Equal("Sport with this name already exists", exception.Message);
        }

        [Fact]
        public async Task UpdateSportAsync_WhenNameIsSameAsCurrentName_UpdatesSuccessfully()
        {
            var existing = SportTestData.ExistingSport();
            var dto = SportTestData.SameNameUpdateDto();

            _mockRepo.Setup(repo => repo.GetSportByIdAsync(1)).ReturnsAsync(existing);
            _mockRepo.Setup(repo => repo.SportExistsAsync("Badminton")).ReturnsAsync(true);
            _mockRepo.Setup(repo => repo.UpdateSportAsync(existing)).Returns(Task.CompletedTask);

            var result = await _service.UpdateSportAsync(1, dto);

            Assert.NotNull(result);
            Assert.Equal("Badminton", result.Name);
            _mockRepo.Verify(repo => repo.UpdateSportAsync(existing), Times.Once);
        }

        [Fact]
        public async Task UpdateSportAsync_WhenValidNewName_UpdatesAndReturnsSuccessfully()
        {
            var existing = SportTestData.ExistingSport();
            var dto = SportTestData.ValidUpdateDto();

            _mockRepo.Setup(repo => repo.GetSportByIdAsync(1)).ReturnsAsync(existing);
            _mockRepo.Setup(repo => repo.SportExistsAsync("Tennis")).ReturnsAsync(false);
            _mockRepo.Setup(repo => repo.UpdateSportAsync(existing)).Returns(Task.CompletedTask);

            var result = await _service.UpdateSportAsync(1, dto);

            Assert.Equal("Tennis", result.Name);
            _mockRepo.Verify(repo => repo.UpdateSportAsync(existing), Times.Once);
        }

        [Fact]
        public async Task GetSportsAsync_ReturnsListFromRepository()
        {
            var expected = SportTestData.SportResponseList();

            _mockRepo
                .Setup(repo => repo.GetSportsAsync(
                    It.IsAny<Expression<Func<Sport, bool>>>(),
                    It.IsAny<Expression<Func<Sport, SportResponseDto>>>()))
                .ReturnsAsync(expected);

            var result = await _service.GetSportsAsync(new Data.Filters.SportFilterDto());

            Assert.Equal(expected.Count, result.Count);
            Assert.Equal(expected[0].Name, result[0].Name);
        }
    }
}
