using AutoMapper;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Data.Filters;
using SportsManagementApp.Data.Predicates;
using SportsManagementApp.Enums;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;
using SportsManagementApp.Helper;
using SportsManagementApp.StringConstants;

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
            throw new ValidationException(StringConstant.SportNotFound);

        var exists = await _eventRequestRepository.ExistsAsync(e =>
            e.SportId == dto.SportId &&
            e.Gender == dto.Gender &&
            e.Format == dto.Format &&
            e.StartDate == dto.StartDate);

        if (exists)
            throw new ConflictException(StringConstant.EventExist);

        var request = _mapper.Map<EventRequest>(dto);
        request.AdminId = adminId;
        request.Status = RequestStatus.Pending;
        request.CreatedDate = DateTime.UtcNow;

        await _eventRequestRepository.AddAsync(request);
        await _eventRequestRepository.SaveChangesAsync();

        var createdRequest = await _eventRequestRepository.GetByIdWithIncludesAsync(
            e => e.Id == request.Id,
            e => e.Sport,
            e => e.OperationsReviewer,
            e => e.Admin
        );

        if (createdRequest == null)
            throw new NotFoundException(StringConstant.NoRequestFound);

        var message = $"New event request #{request.Id} is pending for review.";

        await _notificationService.CreateAsync(
            request.CreateNotification(message, RequestStatus.Pending, NotificationAudience.Ops)
        );

        return _mapper.Map<EventRequestResponseDto>(createdRequest);
    }

    public async Task<EventRequestResponseDto> GetByIdForAdminAsync(int id, int adminId)
    {
        var request = await _eventRequestRepository.GetByIdWithIncludesAsync(
            e => e.Id == id,
            e => e.Sport,
            e => e.OperationsReviewer,
            e => e.Admin
        );

        if (request == null)
            throw new NotFoundException(StringConstant.NoRequestFound);

        if (request.AdminId != adminId)
            throw new ForbiddenException(StringConstant.NoRequestAccess);

        return _mapper.Map<EventRequestResponseDto>(request);
    }

    public async Task<IEnumerable<EventRequestResponseDto>> GetAllEventRequestsAsync(EventRequestFilterDto filter)
    {
        var predicate = EventRequestPredicateBuilder.Build(filter);

        var requests = await _eventRequestRepository.GetAllWithIncludesAsync(
            predicate,
            e => e.Sport,
            e => e.Admin,
            e => e.OperationsReviewer
        );

        return _mapper.Map<List<EventRequestResponseDto>>(requests);
    }

    public async Task<EventRequestResponseDto> EditEventRequestAsync(int id, BaseEventRequestDto dto, int adminId)
    {
        var request = await GetOwnedPendingRequestOrThrowAsync(id, adminId, StringConstant.OnlyEditOwnRequest);

        _mapper.Map(dto, request);
        request.UpdatedDate = DateTime.UtcNow;

        await _eventRequestRepository.UpdateAsync(request);
        await _eventRequestRepository.SaveChangesAsync();

        return _mapper.Map<EventRequestResponseDto>(request);
    }

    public async Task<EventRequestResponseDto> WithdrawEventRequestAsync(int id, int adminId)
    {
        var request = await GetOwnedPendingRequestOrThrowAsync(id, adminId, StringConstant.OnlyWithdrawOwnRequest);

        request.Status = RequestStatus.Withdrawn;
        request.UpdatedDate = DateTime.UtcNow;

        await _eventRequestRepository.UpdateAsync(request);
        await _eventRequestRepository.SaveChangesAsync();

        return _mapper.Map<EventRequestResponseDto>(request);
    }

    public async Task<EventRequestResponseDto> ReviewEventRequestAsync(int id, ReviewEventRequestDto dto, int opsUserId)
    {
        if (dto.Status != RequestStatus.Approved && dto.Status != RequestStatus.Rejected)
            throw new ValidationException(StringConstant.OnlyApproveOrRejectAllowed);

        var request = await _eventRequestRepository.GetByIdWithIncludesAsync(
            e => e.Id == id,
            e => e.Sport,
            e => e.OperationsReviewer,
            e => e.Admin
        );

        if (request == null)
            throw new NotFoundException(StringConstant.NoRequestFound);

        if (request.Status != RequestStatus.Pending)
            throw new ConflictException(StringConstant.RequestProcessNotAllowed);

        request.Status = dto.Status;
        request.Remarks = dto.Remarks?.Trim() ?? string.Empty;
        request.OperationsReviewerId = opsUserId;
        request.UpdatedDate = DateTime.UtcNow;

        await _eventRequestRepository.UpdateAsync(request);
        await _eventRequestRepository.SaveChangesAsync();

        var message = dto.Status == RequestStatus.Approved
            ? $"Your request #{request.Id} has been approved."
            : $"Your request #{request.Id} has been rejected. Remarks: {request.Remarks}";

        await _notificationService.CreateAsync(
            request.CreateNotification(message, dto.Status, NotificationAudience.Admin)
        );

        return _mapper.Map<EventRequestResponseDto>(request);
    }

    private async Task<EventRequest> GetOwnedPendingRequestOrThrowAsync(int id, int adminId, string forbiddenMessage)
    {
        var request = await _eventRequestRepository.GetByIdWithIncludesAsync(
            e => e.Id == id,
            e => e.Sport,
            e => e.OperationsReviewer,
            e => e.Admin
        );

        if (request == null)
            throw new NotFoundException(StringConstant.NoRequestFound);

        if (request.AdminId != adminId)
            throw new ForbiddenException(forbiddenMessage);

        if (request.Status != RequestStatus.Pending)
            throw new ConflictException(StringConstant.EventRequestModifyNotAllowed);

        return request;
    }
}