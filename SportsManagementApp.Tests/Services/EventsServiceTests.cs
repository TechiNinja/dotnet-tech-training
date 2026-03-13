using Moq;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Implementations;
using SportsManagementApp.Tests.TestData;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsManagementApp.Tests.Services
{
    public class EventsServiceTests
    {
        private readonly Mock<IEventsRepository> _mockRepo;
        private readonly EventsService _service;

        public EventsServiceTests()
        {
            _mockRepo = new Mock<IEventsRepository>();
            _service = new EventsService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetUserEventsAsync_WhenUserHasEvents_ReturnsEventsList()
        {
            var expected = EventsTestData.UserEventsList();
            _mockRepo.Setup(repo => repo.GetUserEventsAsync(EventsTestData.ValidUserId))
                     .ReturnsAsync(expected);

            var result = await _service.GetUserEventsAsync(EventsTestData.ValidUserId);

            Assert.Equal(expected.Count, result.Count);
            Assert.Equal(expected[0].EventName, result[0].EventName);
            Assert.Equal(8, result[0].EventId);
            _mockRepo.Verify(repo => repo.GetUserEventsAsync(EventsTestData.ValidUserId), Times.Once);
        }

        [Fact]
        public async Task GetUserEventsAsync_WhenUserHasNoEvents_ReturnsEmptyList()
        {
            var expected = EventsTestData.EmptyEventsList();
            _mockRepo.Setup(repo => repo.GetUserEventsAsync(EventsTestData.NoEventsUserId))
                     .ReturnsAsync(expected);

            var result = await _service.GetUserEventsAsync(EventsTestData.NoEventsUserId);

            Assert.Empty(result);
        }
    }
}
