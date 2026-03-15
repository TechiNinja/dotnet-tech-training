using AutoMapper;
using FluentAssertions;
using Moq;
using SportsManagementApp.Constants;
using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Enums;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Implementations;
using SportsManagementApp.Services.Interfaces;
using SportsManagementApp.Tests.TestData;
using Xunit;

namespace SportsManagementApp.Tests.Services;

public class OperationsServiceTests
{
    private readonly Mock<IOperationsRepository> _operationsRepositoryMock;
    private readonly Mock<IEventRequestRepository> _eventRequestRepositoryMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly OperationsService _service;

    public OperationsServiceTests()
    {
        _operationsRepositoryMock = new Mock<IOperationsRepository>();
        _eventRequestRepositoryMock = new Mock<IEventRequestRepository>();
        _notificationServiceMock = new Mock<INotificationService>();
        _mapperMock = new Mock<IMapper>();

        _service = new OperationsService(
            _operationsRepositoryMock.Object,
            _notificationServiceMock.Object,
            _eventRequestRepositoryMock.Object,
            _mapperMock.Object
        );
    }

    [Fact]
    public async Task DecideAsync_ShouldThrowValidationException_WhenStatusIsInvalid()
    {
        var dto = OperationsServiceTestData.ApprovalDecisionDto;

        Func<Task> act = async () =>
            await _service.DecideAsync(1, dto, 10, RequestStatus.Pending);

        await act.Should()
            .ThrowAsync<ValidationException>()
            .WithMessage(StringConstant.onlyApproveorRejectAllowed);
    }

    [Fact]
    public async Task DecideAsync_ShouldThrowNotFoundException_WhenRequestNotFound()
    {
        var dto = OperationsServiceTestData.ApprovalDecisionDto;

        _eventRequestRepositoryMock
            .Setup(x => x.GetEventRequestByIdAsync(1))
            .ReturnsAsync((EventRequest?)null);

        Func<Task> act = async () =>
            await _service.DecideAsync(1, dto, 10, RequestStatus.Approved);

        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage(StringConstant.noEventFound);
    }

    [Fact]
    public async Task DecideAsync_ShouldThrowConflictException_WhenRequestIsNotPending()
    {
        var dto = OperationsServiceTestData.ApprovalDecisionDto;
        var request = OperationsServiceTestData.ApprovedRequest;
        request.Id = 1;

        _eventRequestRepositoryMock
            .Setup(x => x.GetEventRequestByIdAsync(1))
            .ReturnsAsync(request);

        Func<Task> act = async () =>
            await _service.DecideAsync(1, dto, 10, RequestStatus.Rejected);

        await act.Should()
            .ThrowAsync<ConflictException>()
            .WithMessage(StringConstant.requestProcessNotAllowed);
    }

    [Fact]
    public async Task DecideAsync_ShouldApproveRequest_WhenInputIsValid()
    {
        var dto = OperationsServiceTestData.TrimmedApprovalDecisionDto;
        var request = OperationsServiceTestData.PendingRequestForApproval;
        var expectedResponse = OperationsServiceTestData.SampleResponseDto;

        _eventRequestRepositoryMock
            .Setup(x => x.GetEventRequestByIdAsync(1))
            .ReturnsAsync(request);

        _operationsRepositoryMock
            .Setup(x => x.UpdateAsync(request))
            .Returns(Task.CompletedTask);

        _operationsRepositoryMock
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        _notificationServiceMock
            .Setup(x => x.CreateAsync(
                request.AdminId,
                request.Id,
                $"Your request #{request.Id} has been approved.",
                NotificationType.Approved,
                NotificationAudience.Admin))
            .ReturnsAsync(new Notification());

        _mapperMock
            .Setup(x => x.Map<EventRequestResponseDto>(request))
            .Returns(expectedResponse);

        var result = await _service.DecideAsync(1, dto, 10, RequestStatus.Approved);

        result.Should().BeSameAs(expectedResponse);
        request.Status.Should().Be(RequestStatus.Approved);
        request.Remarks.Should().Be("Looks good");
        request.OperationsReviewerId.Should().Be(10);
        request.UpdatedDate.Should().NotBe(default);

        _operationsRepositoryMock.Verify(x => x.UpdateAsync(request), Times.Once);
        _operationsRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);

        _notificationServiceMock.Verify(x => x.CreateAsync(
            request.AdminId,
            request.Id,
            $"Your request #{request.Id} has been approved.",
            NotificationType.Approved,
            NotificationAudience.Admin), Times.Once);

        _mapperMock.Verify(x => x.Map<EventRequestResponseDto>(request), Times.Once);
    }

    [Fact]
    public async Task DecideAsync_ShouldRejectRequest_WhenInputIsValid()
    {
        var dto = OperationsServiceTestData.TrimmedRejectionDecisionDto;
        var request = OperationsServiceTestData.PendingRequestForRejection;
        var expectedResponse = OperationsServiceTestData.SampleResponseDto;

        _eventRequestRepositoryMock
            .Setup(x => x.GetEventRequestByIdAsync(2))
            .ReturnsAsync(request);

        _operationsRepositoryMock
            .Setup(x => x.UpdateAsync(request))
            .Returns(Task.CompletedTask);

        _operationsRepositoryMock
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        _notificationServiceMock
            .Setup(x => x.CreateAsync(
                request.AdminId,
                request.Id,
                $"Your request #{request.Id} has been rejected. Remarks: {dto.Remarks.Trim()}",
                NotificationType.Rejected,
                NotificationAudience.Admin))
            .ReturnsAsync(new Notification());

        _mapperMock
            .Setup(x => x.Map<EventRequestResponseDto>(request))
            .Returns(expectedResponse);

        var result = await _service.DecideAsync(2, dto, 20, RequestStatus.Rejected);

        result.Should().BeSameAs(expectedResponse);
        request.Status.Should().Be(RequestStatus.Rejected);
        request.Remarks.Should().Be("Slot not available");
        request.OperationsReviewerId.Should().Be(20);
        request.UpdatedDate.Should().NotBe(default);

        _notificationServiceMock.Verify(x => x.CreateAsync(
            request.AdminId,
            request.Id,
            $"Your request #{request.Id} has been rejected. Remarks: {request.Remarks}",
            NotificationType.Rejected,
            NotificationAudience.Admin), Times.Once);
    }

    [Fact]
    public async Task DecideAsync_ShouldTrimRemarks_WhenDecisionIsMade()
    {
        var dto = OperationsServiceTestData.TrimmedOpsApprovalDto;
        var request = OperationsServiceTestData.PendingRequestForTrimCheck;

        _eventRequestRepositoryMock
            .Setup(x => x.GetEventRequestByIdAsync(3))
            .ReturnsAsync(request);

        _operationsRepositoryMock
            .Setup(x => x.UpdateAsync(request))
            .Returns(Task.CompletedTask);

        _operationsRepositoryMock
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        _notificationServiceMock
            .Setup(x => x.CreateAsync(
                It.IsAny<int?>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<NotificationType>(),
                It.IsAny<NotificationAudience>()))
            .ReturnsAsync(new Notification());

        _mapperMock
            .Setup(x => x.Map<EventRequestResponseDto>(request))
            .Returns(new EventRequestResponseDto());

        await _service.DecideAsync(3, dto, 22, RequestStatus.Approved);

        request.Remarks.Should().Be("Approved by ops");
    }

    [Fact]
    public async Task DecideAsync_ShouldSetOperationsReviewerId_WhenDecisionIsMade()
    {
        var dto = OperationsServiceTestData.SimpleDecisionDto;
        var request = OperationsServiceTestData.PendingRequestForReviewerCheck;

        _eventRequestRepositoryMock
            .Setup(x => x.GetEventRequestByIdAsync(4))
            .ReturnsAsync(request);

        _operationsRepositoryMock
            .Setup(x => x.UpdateAsync(request))
            .Returns(Task.CompletedTask);

        _operationsRepositoryMock
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        _notificationServiceMock
            .Setup(x => x.CreateAsync(
                It.IsAny<int?>(),
                It.IsAny<int>(),
                It.IsAny<string>(),
                It.IsAny<NotificationType>(),
                It.IsAny<NotificationAudience>()))
            .ReturnsAsync(new Notification());

        _mapperMock
            .Setup(x => x.Map<EventRequestResponseDto>(request))
            .Returns(new EventRequestResponseDto());

        await _service.DecideAsync(4, dto, 99, RequestStatus.Rejected);

        request.OperationsReviewerId.Should().Be(99);
    }
}