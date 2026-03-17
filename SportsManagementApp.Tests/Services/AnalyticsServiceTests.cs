using Moq;
using SportsManagementApp.Data.DTOs.Analytics;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Implementations;
using SportsManagementApp.Tests.TestData;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace SportsManagementApp.Tests.Services
{
    public class AnalyticsServiceTests
    {
        private readonly Mock<IAnalyticsRepository> _mockRepo;
        private readonly AnalyticsService _service;

        public AnalyticsServiceTests()
        {
            _mockRepo = new Mock<IAnalyticsRepository>();
            _service = new AnalyticsService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetAnalyticsAsync_WhenAdminRole_ReturnsAdminAnalytics()
        {
            var expected = AnalyticsTestData.AdminAnalytics();
            _mockRepo.Setup(repo => repo.GetAdminAnalyticsAsync()).ReturnsAsync(expected);

            var result = await _service.GetAnalyticsAsync(
                AnalyticsTestData.AdminUserId,
                AnalyticsTestData.AdminRole);

            Assert.NotNull(result);
            var adminResult = Assert.IsType<AdminAnalyticsDto>(result);
            Assert.Equal(expected.TotalEvents, adminResult.TotalEvents);
            Assert.Equal(expected.ActiveUsers, adminResult.ActiveUsers);
            _mockRepo.Verify(repo => repo.GetAdminAnalyticsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetAnalyticsAsync_WhenOrganizerRole_ReturnsOrganizerAnalytics()
        {
            var expected = AnalyticsTestData.OrganizerAnalytics();
            _mockRepo.Setup(repo => repo.GetOrganizerAnalyticsAsync(AnalyticsTestData.ValidUserId))
                     .ReturnsAsync(expected);

            var result = await _service.GetAnalyticsAsync(
                AnalyticsTestData.ValidUserId,
                AnalyticsTestData.OrganizerRole);

            Assert.NotNull(result);
            var organizerResult = Assert.IsType<OrganizerAnalyticsDto>(result);
            Assert.Equal(expected.MyEvents, organizerResult.MyEvents);
            Assert.Equal(expected.TotalRegistrations, organizerResult.TotalRegistrations);
            _mockRepo.Verify(repo => repo.GetOrganizerAnalyticsAsync(AnalyticsTestData.ValidUserId), Times.Once);
        }

        [Fact]
        public async Task GetAnalyticsAsync_WhenParticipantRole_ThrowsUnauthorizedException()
        {
            var exception = await Assert.ThrowsAsync<UnauthorizedException>(
                async () => await _service.GetAnalyticsAsync(
                    AnalyticsTestData.ValidUserId,
                    AnalyticsTestData.ParticipantRole));
            Assert.Equal("Analytics not available for this role", exception.Message);
        }

        [Fact]
        public async Task GetAnalyticsAsync_WhenOpsTeamRole_ThrowsUnauthorizedException()
        {
            var exception = await Assert.ThrowsAsync<UnauthorizedException>(
                async () => await _service.GetAnalyticsAsync(
                    AnalyticsTestData.ValidUserId,
                    AnalyticsTestData.OpsTeamRole));
            Assert.Equal("Analytics not available for this role", exception.Message);
        }

        [Fact]
        public async Task GetAnalyticsAsync_WhenInvalidRole_ThrowsUnauthorizedException()
        {
            var exception = await Assert.ThrowsAsync<UnauthorizedException>(
                async () => await _service.GetAnalyticsAsync(
                    AnalyticsTestData.ValidUserId,
                    AnalyticsTestData.InvalidRole));
            Assert.Equal("Analytics not available for this role", exception.Message);
        }

        [Fact]
        public async Task GetAnalyticsAsync_WhenNullRole_ThrowsUnauthorizedException()
        {
            var exception = await Assert.ThrowsAsync<UnauthorizedException>(
                async () => await _service.GetAnalyticsAsync(
                    AnalyticsTestData.ValidUserId,
                    null!));
            Assert.Equal("Analytics not available for this role", exception.Message);
        }

        [Fact]
        public async Task GetAnalyticsAsync_WhenEmptyRole_ThrowsUnauthorizedException()
        {
            var exception = await Assert.ThrowsAsync<UnauthorizedException>(
                async () => await _service.GetAnalyticsAsync(
                    AnalyticsTestData.ValidUserId,
                    string.Empty));
            Assert.Equal("Analytics not available for this role", exception.Message);
        }
    }
}
