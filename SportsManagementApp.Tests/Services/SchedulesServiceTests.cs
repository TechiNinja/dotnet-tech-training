using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Implementations;
using SportsManagementApp.Tests.TestData;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsManagementApp.Tests.Services
{
    public class SchedulesServiceTests
    {
        private readonly Mock<ISchedulesRepository> _mockRepo;
        private readonly SchedulesService _service;

        public SchedulesServiceTests()
        {
            _mockRepo = new Mock<ISchedulesRepository>();
            _service = new SchedulesService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetUserSchedulesAsync_WhenUserHasSchedules_ReturnsSchedulesList()
        {
            var expected = SchedulesTestData.UserSchedulesList();
            _mockRepo.Setup(repo => repo.GetUserScheduleAsync(SchedulesTestData.ValidUserId))
                     .ReturnsAsync(expected);

            var result = await _service.GetUserSchedulesAsync(SchedulesTestData.ValidUserId);

            Assert.Equal(expected.Count, result.Count);
            Assert.Equal(expected[0].EventName, result[0].EventName);
            Assert.Equal(expected[0].Venue, result[0].Venue);
        }

        [Fact]
        public async Task GetUserSchedulesAsync_WhenUserHasNoSchedules_ReturnsEmptyList()
        {
            var expected = SchedulesTestData.EmptySchedulesList();
            _mockRepo.Setup(repo => repo.GetUserScheduleAsync(SchedulesTestData.NoScheduleUserId))
                     .ReturnsAsync(expected);

            var result = await _service.GetUserSchedulesAsync(SchedulesTestData.NoScheduleUserId);

            Assert.Empty(result);
        }
    }
}
