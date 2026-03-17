using SportsManagementApp.Data.Entities;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Implementations;
using SportsManagementApp.Tests.TestData;
using Moq;
using AutoMapper;
using SportsManagementApp.Data.DTOs.Participant;
using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace SportsManagementApp.Tests.Services
{
    public class ParticipantRegistrationServiceTests
    {
        private readonly Mock<IParticipantRegistrationRepository> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ParticipantRegistrationService _service;

        public ParticipantRegistrationServiceTests()
        {
            _mockRepo = new Mock<IParticipantRegistrationRepository>();
            _mockMapper = new Mock<IMapper>();
            _service = new ParticipantRegistrationService(_mockRepo.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task RegisterParticipantAsync_WhenAlreadyRegistered_ThrowsConflictException()
        {
            var request = ParticipantRegistrationTestData.DuplicateRegistrationRequest();
            _mockRepo.Setup(repo => repo.IsUserRegisteredInCategoryAsync(request.UserId, request.EventCategoryId))
                     .ReturnsAsync(true);

            var exception = await Assert.ThrowsAsync<ConflictException>(
                async () => await _service.RegisterParticipantAsync(request));
            Assert.Equal("Participant already registered in this category", exception.Message);
        }

        [Fact]
        public async Task RegisterParticipantAsync_WhenRegistrationNotSavedCorrectly_ThrowsBadRequestException()
        {
            var request = ParticipantRegistrationTestData.ValidRegistrationRequest();
            var mappedRegistration = ParticipantRegistrationTestData.MappedRegistration();

            _mockRepo.Setup(repo => repo.IsUserRegisteredInCategoryAsync(request.UserId, request.EventCategoryId))
                     .ReturnsAsync(false);
            _mockMapper.Setup(mapper => mapper.Map<ParticipantRegistration>(request)).Returns(mappedRegistration);
            _mockRepo.Setup(repo => repo.AddAsync(mappedRegistration)).Returns(Task.CompletedTask);
            _mockRepo.Setup(repo => repo.GetParticipantsByIdWithUserAsync(mappedRegistration.Id))
                     .ReturnsAsync((ParticipantRegistration?)null);

            var exception = await Assert.ThrowsAsync<BadRequestException>(
                async () => await _service.RegisterParticipantAsync(request));
            Assert.Equal("Registration not saved correctly", exception.Message);
        }

        [Fact]
        public async Task RegisterParticipantAsync_WhenValidRequest_ReturnsSuccessfulRegistration()
        {
            var request = ParticipantRegistrationTestData.ValidRegistrationRequest();
            var mappedRegistration = ParticipantRegistrationTestData.MappedRegistration();
            var savedRegistration = ParticipantRegistrationTestData.SavedRegistrationWithUser();
            var expectedResponse = ParticipantRegistrationTestData.SuccessfulRegistrationResponse();

            _mockRepo.Setup(repo => repo.IsUserRegisteredInCategoryAsync(request.UserId, request.EventCategoryId))
                     .ReturnsAsync(false);
            _mockMapper.Setup(mapper => mapper.Map<ParticipantRegistration>(request)).Returns(mappedRegistration);
            _mockRepo.Setup(repo => repo.AddAsync(mappedRegistration)).Returns(Task.CompletedTask);
            _mockRepo.Setup(repo => repo.GetParticipantsByIdWithUserAsync(mappedRegistration.Id))
                     .ReturnsAsync(savedRegistration);
            _mockMapper.Setup(mapper => mapper.Map<ParticipantRegistrationResponseDto>(savedRegistration))
                       .Returns(expectedResponse);

            var result = await _service.RegisterParticipantAsync(request);

            Assert.NotNull(result);
            Assert.Equal(expectedResponse.Id, result.Id);
            Assert.Equal(expectedResponse.Name, result.Name);
            _mockRepo.Verify(repo => repo.AddAsync(mappedRegistration), Times.Once);
        }

        [Fact]
        public async Task GetRegistrationsByCategoryAsync_WhenCategoryHasRegistrations_ReturnsRegistrationsList()
        {
            var registrations = ParticipantRegistrationTestData.CategoryRegistrationsList();
            var expected = ParticipantRegistrationTestData.CategoryRegistrationsResponseList();
            _mockRepo.Setup(repo => repo.GetParticipantsByCategoryAsync(ParticipantRegistrationTestData.ValidCategoryId))
                     .ReturnsAsync(registrations);
            _mockMapper.Setup(mapper => mapper.Map<List<ParticipantRegistrationResponseDto>>(registrations))
                       .Returns(expected);

            var result = await _service.GetRegistrationsByCategoryAsync(ParticipantRegistrationTestData.ValidCategoryId);

            Assert.Equal(expected.Count, result.Count);
            Assert.Equal(expected[0].Name, result[0].Name);
            Assert.Equal(expected[1].Name, result[1].Name);
        }

        [Fact]
        public async Task GetRegistrationsByCategoryAsync_WhenCategoryHasNoRegistrations_ReturnsEmptyList()
        {
            var registrations = new List<ParticipantRegistration>();
            var expected = ParticipantRegistrationTestData.EmptyRegistrationsResponseList();
            _mockRepo.Setup(repo => repo.GetParticipantsByCategoryAsync(ParticipantRegistrationTestData.EmptyCategoryId))
                     .ReturnsAsync(registrations);
            _mockMapper.Setup(mapper => mapper.Map<List<ParticipantRegistrationResponseDto>>(registrations))
                       .Returns(expected);

            var result = await _service.GetRegistrationsByCategoryAsync(ParticipantRegistrationTestData.EmptyCategoryId);

            Assert.Empty(result);
        }
    }
}
