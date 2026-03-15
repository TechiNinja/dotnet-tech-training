using System.Linq.Expressions;
using AutoMapper;
using FluentAssertions;
using Moq;
using SportsManagementApp.Constants;
using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Data.Filters;
using SportsManagementApp.Enums;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Implementations;
using SportsManagementApp.Services.Interfaces;
using SportsManagementApp.Tests.TestData;
using Xunit;

namespace SportsManagementApp.Tests.Services;

public class EventRequestServiceTests
{
    private readonly Mock<IEventRequestRepository> _eventRequestRepositoryMock;
    private readonly Mock<ISportRepository> _sportRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly EventRequestService _service;

    public EventRequestServiceTests()
    {
        _eventRequestRepositoryMock = new Mock<IEventRequestRepository>();
        _sportRepositoryMock = new Mock<ISportRepository>();
        _mapperMock = new Mock<IMapper>();
        _notificationServiceMock = new Mock<INotificationService>();

        _service = new EventRequestService(
            _eventRequestRepositoryMock.Object,
            _sportRepositoryMock.Object,
            _mapperMock.Object,
            _notificationServiceMock.Object
        );
    }

    #region RaiseEventRequestAsync Tests

    [Fact]
    public async Task RaiseEventRequestAsync_ShouldThrowValidationException_WhenStartDateIsAfterEndDate()
    {
        var dto = EventRequestTestData.InvalidDateDto;

        Func<Task> act = async () => await _service.RaiseEventRequestAsync(dto, 10);

        await act.Should()
            .ThrowAsync<ValidationException>()
            .WithMessage(StringConstant.DateCompare);
    }

    [Fact]
    public async Task RaiseEventRequestAsync_ShouldThrowValidationException_WhenSportDoesNotExist()
    {
        var dto = EventRequestTestData.ValidCreateDto;

        _sportRepositoryMock
            .Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Sport, bool>>>()))
            .ReturnsAsync(false);

        Func<Task> act = async () => await _service.RaiseEventRequestAsync(dto, 10);

        await act.Should()
            .ThrowAsync<ValidationException>()
            .WithMessage(StringConstant.sportNotFound);
    }

    [Fact]
    public async Task RaiseEventRequestAsync_ShouldThrowConflictException_WhenDuplicateRequestExists()
    {
        var dto = EventRequestTestData.DuplicateEventDto;

        _sportRepositoryMock
            .Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Sport, bool>>>()))
            .ReturnsAsync(true);

        _eventRequestRepositoryMock
            .Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<EventRequest, bool>>>()))
            .ReturnsAsync(true);

        Func<Task> act = async () => await _service.RaiseEventRequestAsync(dto, 10);

        await act.Should()
            .ThrowAsync<ConflictException>()
            .WithMessage(StringConstant.eventExist);
    }

    [Fact]
    public async Task RaiseEventRequestAsync_ShouldCreatePendingRequest_WhenInputIsValid()
    {
        var dto = EventRequestTestData.ValidCreateDto;
        var mappedRequest = EventRequestTestData.SampleEventRequest;
        var createdDto = EventRequestTestData.SampleResponseDto;

        _sportRepositoryMock
            .Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Sport, bool>>>()))
            .ReturnsAsync(true);

        _eventRequestRepositoryMock
            .Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<EventRequest, bool>>>()))
            .ReturnsAsync(false);

        _mapperMock
            .Setup(x => x.Map<EventRequest>(dto))
            .Returns(mappedRequest);

        _eventRequestRepositoryMock
            .Setup(x => x.AddAsync(mappedRequest))
            .Returns(Task.CompletedTask);

        _eventRequestRepositoryMock
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        _eventRequestRepositoryMock
            .Setup(x => x.GetEventRequestDtoByIdAsync(mappedRequest.Id))
            .ReturnsAsync(createdDto);

        _notificationServiceMock
            .Setup(x => x.CreateAsync(
                null,
                mappedRequest.Id,
                $"New event request #{mappedRequest.Id} is pending for review.",
                NotificationType.NewRequest,
                NotificationAudience.Ops))
            .ReturnsAsync(new Notification());

        var result = await _service.RaiseEventRequestAsync(dto, 25);

        result.Should().BeSameAs(createdDto);
        mappedRequest.AdminId.Should().Be(25);
        mappedRequest.Status.Should().Be(RequestStatus.Pending);
        mappedRequest.CreatedDate.Should().NotBe(default);

        _eventRequestRepositoryMock.Verify(x => x.AddAsync(mappedRequest), Times.Once);
        _eventRequestRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        _notificationServiceMock.Verify(x => x.CreateAsync(
            null,
            mappedRequest.Id,
            $"New event request #{mappedRequest.Id} is pending for review.",
            NotificationType.NewRequest,
            NotificationAudience.Ops), Times.Once);
    }

    [Fact]
    public async Task RaiseEventRequestAsync_ShouldThrowNotFoundException_WhenCreatedRequestDtoIsNull()
    {
        var dto = EventRequestTestData.ValidCreateDto;
        var mappedRequest = EventRequestTestData.SampleEventRequest;

        _sportRepositoryMock
            .Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<Sport, bool>>>()))
            .ReturnsAsync(true);

        _eventRequestRepositoryMock
            .Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<EventRequest, bool>>>()))
            .ReturnsAsync(false);

        _mapperMock
            .Setup(x => x.Map<EventRequest>(dto))
            .Returns(mappedRequest);

        _eventRequestRepositoryMock
            .Setup(x => x.AddAsync(mappedRequest))
            .Returns(Task.CompletedTask);

        _eventRequestRepositoryMock
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        _eventRequestRepositoryMock
            .Setup(x => x.GetEventRequestDtoByIdAsync(mappedRequest.Id))
            .ReturnsAsync((EventRequestResponseDto?)null);

        Func<Task> act = async () => await _service.RaiseEventRequestAsync(dto, 25);

        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage(StringConstant.noRequestFound);
    }

    #endregion

    #region GetByIdForAdminAsync Tests

    [Fact]
    public async Task GetByIdForAdminAsync_ShouldThrowNotFoundException_WhenRequestDoesNotExist()
    {
        _eventRequestRepositoryMock
            .Setup(x => x.GetEventRequestDtoByIdAsync(1))
            .ReturnsAsync((EventRequestResponseDto?)null);

        Func<Task> act = async () => await _service.GetByIdForAdminAsync(1, 5);

        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage(StringConstant.noRequestFound);
    }

    [Fact]
    public async Task GetByIdForAdminAsync_ShouldThrowForbiddenException_WhenAdminDoesNotOwnRequest()
    {
        var response = EventRequestTestData.SampleResponseDto;
        response.AdminId = 99;

        _eventRequestRepositoryMock
            .Setup(x => x.GetEventRequestDtoByIdAsync(1))
            .ReturnsAsync(response);

        Func<Task> act = async () => await _service.GetByIdForAdminAsync(1, 5);

        await act.Should()
            .ThrowAsync<ForbiddenException>()
            .WithMessage(StringConstant.noRequestAccess);
    }

    [Fact]
    public async Task GetByIdForAdminAsync_ShouldReturnRequest_WhenAdminOwnsRequest()
    {
        var response = EventRequestTestData.SampleResponseDto;
        response.AdminId = 5;

        _eventRequestRepositoryMock
            .Setup(x => x.GetEventRequestDtoByIdAsync(1))
            .ReturnsAsync(response);

        var result = await _service.GetByIdForAdminAsync(1, 5);

        result.Should().BeSameAs(response);
    }

    #endregion

    #region SearchEventRequestsAsync Tests

    [Fact]
    public async Task SearchEventRequestsAsync_ShouldReturnFilteredRequests()
    {
        var filter = new EventRequestFilterDto();
        var expected = new List<EventRequestResponseDto>
        {
            EventRequestTestData.SampleResponseDto,
            EventRequestTestData.SampleResponseDto
        };

        _eventRequestRepositoryMock
            .Setup(x => x.GetEventRequestsByFilterAsync(filter))
            .ReturnsAsync(expected);

        var result = await _service.GetAllEventRequestsAsync(filter);

        result.Should().BeEquivalentTo(expected);
    }

    #endregion

    #region EditEventRequestAsync Tests

    [Fact]
    public async Task EditEventRequestAsync_ShouldThrowNotFoundException_WhenRequestDoesNotExist()
    {
        var dto = EventRequestTestData.EditDto;

        _eventRequestRepositoryMock
            .Setup(x => x.GetEventRequestByIdAsync(1))
            .ReturnsAsync((EventRequest?)null);

        Func<Task> act = async () => await _service.EditEventRequestAsync(1, dto, 10);

        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage(StringConstant.noRequestFound);
    }

    [Fact]
    public async Task EditEventRequestAsync_ShouldThrowForbiddenException_WhenAdminTriesToEditAnotherUsersRequest()
    {
        var dto = EventRequestTestData.EditDto;
        var request = EventRequestTestData.SampleEventRequest;
        request.AdminId = 99;
        request.Status = RequestStatus.Pending;

        _eventRequestRepositoryMock
            .Setup(x => x.GetEventRequestByIdAsync(1))
            .ReturnsAsync(request);

        Func<Task> act = async () => await _service.EditEventRequestAsync(1, dto, 10);

        await act.Should()
            .ThrowAsync<ForbiddenException>()
            .WithMessage(StringConstant.onlyEditOwnRequest);
    }

    [Fact]
    public async Task EditEventRequestAsync_ShouldThrowConflictException_WhenRequestIsNotPending()
    {
        var dto = EventRequestTestData.EditDto;
        var request = EventRequestTestData.SampleEventRequest;
        request.AdminId = 10;
        request.Status = RequestStatus.Approved;

        _eventRequestRepositoryMock
            .Setup(x => x.GetEventRequestByIdAsync(1))
            .ReturnsAsync(request);

        Func<Task> act = async () => await _service.EditEventRequestAsync(1, dto, 10);

        await act.Should()
            .ThrowAsync<ConflictException>()
            .WithMessage(StringConstant.eventRequestModifyNotAllowed);
    }

    [Fact]
    public async Task EditEventRequestAsync_ShouldUpdateRequest_WhenInputIsValid()
    {
        var dto = EventRequestTestData.EditDto;
        var request = EventRequestTestData.SampleEventRequest;
        request.AdminId = 10;
        request.Status = RequestStatus.Pending;
        var mappedResponse = EventRequestTestData.SampleResponseDto;

        _eventRequestRepositoryMock
            .Setup(x => x.GetEventRequestByIdAsync(1))
            .ReturnsAsync(request);

        _mapperMock
            .Setup(x => x.Map(dto, request));

        _eventRequestRepositoryMock
            .Setup(x => x.UpdateAsync(request))
            .Returns(Task.CompletedTask);

        _eventRequestRepositoryMock
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        _mapperMock
            .Setup(x => x.Map<EventRequestResponseDto>(request))
            .Returns(mappedResponse);

        var result = await _service.EditEventRequestAsync(1, dto, 10);

        result.Should().BeSameAs(mappedResponse);
        request.UpdatedDate.Should().NotBe(default);
    }

    #endregion

    #region WithdrawEventRequestAsync Tests

    [Fact]
    public async Task WithdrawEventRequestAsync_ShouldThrowNotFoundException_WhenRequestDoesNotExist()
    {
        _eventRequestRepositoryMock
            .Setup(x => x.GetEventRequestByIdAsync(1))
            .ReturnsAsync((EventRequest?)null);

        Func<Task> act = async () => await _service.WithdrawEventRequestAsync(1, 10);

        await act.Should()
            .ThrowAsync<NotFoundException>()
            .WithMessage(StringConstant.noRequestFound);
    }

    [Fact]
    public async Task WithdrawEventRequestAsync_ShouldThrowForbiddenException_WhenAdminTriesToWithdrawAnotherUsersRequest()
    {
        var request = EventRequestTestData.SampleEventRequest;
        request.Id = 1;
        request.AdminId = 99;
        request.Status = RequestStatus.Pending;

        _eventRequestRepositoryMock
            .Setup(x => x.GetEventRequestByIdAsync(1))
            .ReturnsAsync(request);

        Func<Task> act = async () => await _service.WithdrawEventRequestAsync(1, 10);

        await act.Should()
            .ThrowAsync<ForbiddenException>()
            .WithMessage(StringConstant.onlyWithdrawOwnRequest);
    }

    [Fact]
    public async Task WithdrawEventRequestAsync_ShouldThrowConflictException_WhenRequestIsNotPending()
    {
        var request = EventRequestTestData.SampleEventRequest;
        request.Id = 1;
        request.AdminId = 10;
        request.Status = RequestStatus.Rejected;

        _eventRequestRepositoryMock
            .Setup(x => x.GetEventRequestByIdAsync(1))
            .ReturnsAsync(request);

        Func<Task> act = async () => await _service.WithdrawEventRequestAsync(1, 10);

        await act.Should()
            .ThrowAsync<ConflictException>()
            .WithMessage(StringConstant.eventRequestWithdrawlNotAllowed);
    }

    [Fact]
    public async Task WithdrawEventRequestAsync_ShouldMarkAsWithdrawn_WhenInputIsValid()
    {
        var request = EventRequestTestData.SampleEventRequest;
        request.Id = 1;
        request.AdminId = 10;
        request.Status = RequestStatus.Pending;
        var mappedResponse = EventRequestTestData.SampleResponseDto;

        _eventRequestRepositoryMock
            .Setup(x => x.GetEventRequestByIdAsync(1))
            .ReturnsAsync(request);

        _eventRequestRepositoryMock
            .Setup(x => x.UpdateAsync(request))
            .Returns(Task.CompletedTask);

        _eventRequestRepositoryMock
            .Setup(x => x.SaveChangesAsync())
            .ReturnsAsync(1);

        _mapperMock
            .Setup(x => x.Map<EventRequestResponseDto>(request))
            .Returns(mappedResponse);

        var result = await _service.WithdrawEventRequestAsync(1, 10);

        result.Should().BeSameAs(mappedResponse);
        request.Status.Should().Be(RequestStatus.Withdrawn);
        request.UpdatedDate.Should().NotBe(default);
    }

    #endregion
}