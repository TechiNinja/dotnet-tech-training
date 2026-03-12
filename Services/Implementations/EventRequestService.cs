using AutoMapper;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Data.Filters;
using SportsManagementApp.Enums;
using SportsManagementApp.Helper;
using SportsManagementApp.Constants;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Services.Implementations;

public class EventRequestService : IEventRequestService
{
    private readonly IEventRequestRepository _eventRequestRepository;
    private readonly ISportRepository _sportRepository;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;

    public EventRequestService(
        IEventRequestRepository eventRequestRepository,
        ISportRepository sportRepository,
        IMapper mapper,
        INotificationService notificationService)
    {
        _eventRequestRepository = eventRequestRepository;
        _sportRepository = sportRepository;
        _mapper = mapper;
        _notificationService = notificationService;
    }

    public async Task<EventRequestResponseDto> RaiseEventRequestAsync(CreateEventRequestDto dto, int adminId)
    {
        ValidateDates(dto.StartDate, dto.EndDate);

        var sportExists = await _sportRepository.ExistsAsync(sport => sport.Id == dto.SportId);
        if (!sportExists)
            throw new ValidationException("Selected sport does not exist.");

        var exists = await _eventRequestRepository.ExistsAsync(e =>
            e.SportId == dto.SportId &&
            e.Gender == dto.Gender &&
            e.Format == dto.Format &&
            e.StartDate == dto.StartDate);

        if (exists)
            throw new ConflictException(StringConstant.eventExist);

        var request = _mapper.Map<EventRequest>(dto);
        request.AdminId = adminId;
        request.Status = RequestStatus.Pending;
        request.CreatedDate = DateTime.UtcNow;

        await _eventRequestRepository.AddAsync(request);
        await _eventRequestRepository.SaveChangesAsync();

        var createdRequest = await _eventRequestRepository.GetEventRequestDtoByIdAsync(request.Id);
        if (createdRequest == null)
            throw new NotFoundException(StringConstant.noRequestFound);

        await _notificationService.CreateAsync(
            userId: null,
            requestId: request.Id,
            message: $"New event request #{request.Id} is pending for review.",
            type: NotificationType.NewRequest,
            audience: NotificationAudience.Ops
        );

        return createdRequest;
    }

    public async Task<EventRequestResponseDto> GetByIdForAdminAsync(int id, int adminId)
    {
        var request = await _eventRequestRepository.GetEventRequestDtoByIdAsync(id);

        if (request == null)
            throw new NotFoundException(StringConstant.noRequestFound);

        if (request.AdminId != adminId)
            throw new ForbiddenException("You can only access your own event requests.");

        return request;
    }

    public async Task<IEnumerable<EventRequestResponseDto>> SearchEventRequestsAsync(EventRequestFilterDto filter)
    {
        return await _eventRequestRepository.GetEventRequestsByFilterAsync(filter);
    }

    public async Task<EventRequestResponseDto> EditEventRequestAsync(int id, EditEventRequestDto dto, int adminId)
    {
        ValidateDates(dto.StartDate, dto.EndDate);

        var request = await _eventRequestRepository.GetEventRequestByIdAsync(id);

        if (request == null)
            throw new NotFoundException(StringConstant.noRequestFound);

        if (request.AdminId != adminId)
            throw new ForbiddenException("You can only edit your own event requests.");

        if (request.Status != RequestStatus.Pending)
            throw new ConflictException(StringConstant.eventRequestModifyNotAllowed);

        _mapper.Map(dto, request);
        request.UpdatedDate = DateTime.UtcNow;

        _eventRequestRepository.UpdateAsync(request);
        await _eventRequestRepository.SaveChangesAsync();

        return _mapper.Map<EventRequestResponseDto>(request);
    }

    public async Task<EventRequestResponseDto> WithdrawEventRequestAsync(int id, int adminId)
    {
        var request = await _eventRequestRepository.GetEventRequestByIdAsync(id);

        if (request == null)
            throw new NotFoundException(StringConstant.noRequestFound);

        if (request.AdminId != adminId)
            throw new ForbiddenException("You can only withdraw your own event requests.");

        if (request.Status != RequestStatus.Pending)
            throw new ConflictException(StringConstant.eventRequestWithdrawlNotAllowed);

        request.Status = RequestStatus.Withdrawn;
        request.UpdatedDate = DateTime.UtcNow;

        _eventRequestRepository.UpdateAsync(request);
        await _eventRequestRepository.SaveChangesAsync();

        return _mapper.Map<EventRequestResponseDto>(request);
    }

    private static void ValidateDates(DateOnly start, DateOnly end)
    {
        if (start > end)
            throw new ValidationException(StringConstant.DateCompare);
    }

}