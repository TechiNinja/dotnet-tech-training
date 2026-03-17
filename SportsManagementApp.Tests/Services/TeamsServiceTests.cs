using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Implementations;
using AutoMapper;
using Moq;
using SportsManagementApp.Tests.TestData;
using System.Linq.Expressions;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Data.DTOs.TeamManagement;
using SportsManagementApp.Data.Filters;
using SportsManagementApp.Exceptions;

namespace SportsManagementApp.Tests.Services
{
    public class TeamsServiceTests
    {
        private readonly Mock<IGenericRepository<Team>> _mockTeamsRepo;
        private readonly Mock<IParticipantRegistrationRepository> _mockParticipantRegistrationRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly TeamsService _service;

        public TeamsServiceTests()
        {
            _mockTeamsRepo = new Mock<IGenericRepository<Team>>();
            _mockParticipantRegistrationRepo = new Mock<IParticipantRegistrationRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new TeamsService(
                _mockTeamsRepo.Object,
                _mockParticipantRegistrationRepo.Object,
                _mockMapper.Object
            );
        }

        [Fact]
        public async Task GetTeamsAsync_ReturnsTeamsList()
        {
            var expected = TeamTestData.TeamResponseList();
            _mockTeamsRepo.Setup(repo => repo.GetAllAsync(
                It.IsAny<Expression<Func<Team, bool>>>(),
                It.IsAny<Expression<Func<Team, TeamResponseDto>>>()))
                .ReturnsAsync(expected);

            var result = await _service.GetTeamsAsync(new TeamFilterDto());
            Assert.Equal(expected.Count, result.Count);
            Assert.Equal(expected[0].Name, result[0].Name);
        }

        [Fact]
        public async Task CreateTeamsAsync_WhenNoParticipants_ThrowsBadRequestException()
        {
            var request = TeamTestData.ValidCreateTeamRequest();
            _mockParticipantRegistrationRepo.Setup(repo => repo.GetParticipantsByCategoryAsync(request.EventCategoryId))
                .ReturnsAsync(TeamTestData.NoParticipants());

            var exception = await Assert.ThrowsAsync<BadRequestException>(
                async () => await _service.CreateTeamsAsync(request));
            Assert.Equal("Not enough participants to form teams", exception.Message);
        }

        [Fact]
        public async Task CreateTeamsAsync_WhenOneParticipant_ThrowsBadRequestException()
        {
            var request = TeamTestData.ValidCreateTeamRequest();
            _mockParticipantRegistrationRepo.Setup(repo => repo.GetParticipantsByCategoryAsync(request.EventCategoryId))
                .ReturnsAsync(TeamTestData.OneParticipant());

            var exception = await Assert.ThrowsAsync<BadRequestException>(
                async () => await _service.CreateTeamsAsync(request));
            Assert.Equal("Not enough participants to form teams", exception.Message);
        }

        [Fact]
        public async Task CreateTeamsAsync_WhenTwoParticipants_CreatesOneTeam()
        {
            var request = TeamTestData.ValidCreateTeamRequest();
            _mockParticipantRegistrationRepo.Setup(repo => repo.GetParticipantsByCategoryAsync(request.EventCategoryId))
                .ReturnsAsync(TeamTestData.TwoParticipants());

            _mockMapper.Setup(mapper => mapper.Map<Team>(It.IsAny<object>()))
                .Returns(new Team { EventCategoryId = 8 });
            _mockMapper.Setup(mapper => mapper.Map<TeamResponseDto>(It.IsAny<object>()))
                .Returns(new TeamResponseDto { Name = "Team 1", EventCategoryId = 8 });
            _mockTeamsRepo.Setup(repo => repo.AddAsync(It.IsAny<Team>())).Returns(Task.CompletedTask);
            _mockTeamsRepo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _service.CreateTeamsAsync(request);

            Assert.Single(result);
            _mockTeamsRepo.Verify(repo => repo.AddAsync(It.IsAny<Team>()), Times.Once);
            _mockTeamsRepo.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateTeamsAsync_WhenFourParticipants_CreatesTwoTeams()
        {
            var request = TeamTestData.ValidCreateTeamRequest();
            _mockParticipantRegistrationRepo.Setup(repo => repo.GetParticipantsByCategoryAsync(request.EventCategoryId))
                .ReturnsAsync(TeamTestData.FourParticipants());

            _mockMapper.Setup(mapper => mapper.Map<Team>(It.IsAny<object>()))
                .Returns(() => new Team { EventCategoryId = 8 });
            _mockMapper.Setup(mapper => mapper.Map<TeamResponseDto>(It.IsAny<object>()))
                .Returns(new TeamResponseDto { EventCategoryId = 8 });
            _mockTeamsRepo.Setup(repo => repo.AddAsync(It.IsAny<Team>())).Returns(Task.CompletedTask);
            _mockTeamsRepo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _service.CreateTeamsAsync(request);

            Assert.Equal(2, result.Count);
            _mockTeamsRepo.Verify(repo => repo.AddAsync(It.IsAny<Team>()), Times.Exactly(2));
            _mockTeamsRepo.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateTeamsAsync_WhenOddParticipants_IgnoresLastAndCreatesTeam()
        {
            var request = TeamTestData.ValidCreateTeamRequest();
            _mockParticipantRegistrationRepo.Setup(repo => repo.GetParticipantsByCategoryAsync(request.EventCategoryId))
                .ReturnsAsync(TeamTestData.ThreeParticipants());

            _mockMapper.Setup(mapper => mapper.Map<Team>(It.IsAny<object>()))
                .Returns(() => new Team { EventCategoryId = 8 });
            _mockMapper.Setup(mapper => mapper.Map<TeamResponseDto>(It.IsAny<object>()))
                .Returns(new TeamResponseDto { Name = "Team 1", EventCategoryId = 8 });
            _mockTeamsRepo.Setup(repo => repo.AddAsync(It.IsAny<Team>())).Returns(Task.CompletedTask);
            _mockTeamsRepo.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

            var result = await _service.CreateTeamsAsync(request);

            Assert.Single(result);
            _mockTeamsRepo.Verify(repo => repo.AddAsync(It.IsAny<Team>()), Times.Once);
            _mockTeamsRepo.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }
    }
}
