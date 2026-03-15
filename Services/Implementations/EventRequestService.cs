using AutoMapper;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Constants;
using SportsManagementApp.Data.Filters;
using SportsManagementApp.Enums;
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
        if (dto.StartDate > dto.EndDate)
            throw new ValidationException(StringConstant.DateCompare);

        var sportExists = await _sportRepository.ExistsAsync(sport => sport.Id == dto.SportId);
        if (!sportExists)
            throw new ValidationException(StringConstant.sportNotFound);

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
            throw new ForbiddenException(StringConstant.noRequestAccess);

        return request;
    }
    public async Task<IEnumerable<EventRequestResponseDto>> GetAllEventRequestsAsync(EventRequestFilterDto filter)
    {
        return await _eventRequestRepository.GetEventRequestsByFilterAsync(filter);
    }

    public async Task<EventRequestResponseDto> EditEventRequestAsync(int id, EditEventRequestDto dto, int adminId)
    {
        var request = await GetOwnedPendingRequestOrThrowAsync(id, adminId, StringConstant.onlyEditOwnRequest);

        _mapper.Map(dto, request);
        request.UpdatedDate = DateTime.UtcNow;

        await _eventRequestRepository.UpdateAsync(request);
        await _eventRequestRepository.SaveChangesAsync();

        return _mapper.Map<EventRequestResponseDto>(request);
    }

    public async Task<EventRequestResponseDto> WithdrawEventRequestAsync(int id, int adminId)
    {
        var request = await GetOwnedPendingRequestOrThrowAsync(id, adminId, StringConstant.onlyWithdrawOwnRequest);

        request.Status = RequestStatus.Withdrawn;
        request.UpdatedDate = DateTime.UtcNow;

        await _eventRequestRepository.UpdateAsync(request);
        await _eventRequestRepository.SaveChangesAsync();

        return _mapper.Map<EventRequestResponseDto>(request);
    }

    private async Task<EventRequest> GetOwnedPendingRequestOrThrowAsync(int id, int adminId, string forbiddenMessage)
    {
        var request = await _eventRequestRepository.GetEventRequestByIdAsync(id);

        if (request == null)
            throw new NotFoundException(StringConstant.noRequestFound);

        if (request.AdminId != adminId)
            throw new ForbiddenException(forbiddenMessage);

        if (request.Status != RequestStatus.Pending)
            throw new ConflictException(StringConstant.eventRequestModifyNotAllowed);

        return request;
    }

}