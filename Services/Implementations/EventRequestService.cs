using AutoMapper;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Data.Entities;
<<<<<<< HEAD
=======
using SportsManagementApp.Constants;
>>>>>>> 3589b78 (Resolve the PR comments and only the specific file for review)
using SportsManagementApp.Data.Filters;
using SportsManagementApp.Enums;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;
using SportsManagementApp.Helper;
<<<<<<< HEAD
using SportsManagementApp.StringConstants;
=======
>>>>>>> 3589b78 (Resolve the PR comments and only the specific file for review)

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
<<<<<<< HEAD
            throw new ValidationException(StringConstant.SportNotFound);
=======
            throw new ValidationException(StringConstant.sportNotFound);
>>>>>>> 3589b78 (Resolve the PR comments and only the specific file for review)

        var exists = await _eventRequestRepository.ExistsAsync(e =>
            e.SportId == dto.SportId &&
            e.Gender == dto.Gender &&
            e.Format == dto.Format &&
            e.StartDate == dto.StartDate);

        if (exists)
<<<<<<< HEAD
            throw new ConflictException(StringConstant.EventExist);
=======
            throw new ConflictException(StringConstant.eventExist);
>>>>>>> 3589b78 (Resolve the PR comments and only the specific file for review)

        var request = _mapper.Map<EventRequest>(dto);
        request.AdminId = adminId;
        request.Status = RequestStatus.Pending;
        request.CreatedDate = DateTime.UtcNow;

        await _eventRequestRepository.AddAsync(request);
        await _eventRequestRepository.SaveChangesAsync();

<<<<<<< HEAD
        var createdRequest = await _eventRequestRepository.GetEventRequestByIdAsync(request.Id);
        if (createdRequest == null)
            throw new NotFoundException(StringConstant.NoRequestFound);

        var message = $"New event request #{request.Id} is pending for review.";

        await _notificationService.CreateAsync(
            request.CreateNotification(message, RequestStatus.Pending, NotificationAudience.Ops)
        );
=======
        var createdRequest = await _eventRequestRepository.GetEventRequestDtoByIdAsync(request.Id);
        if (createdRequest == null)
            throw new NotFoundException(StringConstant.noRequestFound);
>>>>>>> 3589b78 (Resolve the PR comments and only the specific file for review)

        var message = $"New event request #{request.Id} is pending for review.";    

        await _notificationService.CreateAsync(
        request.CreateNotification(message, RequestStatus.Pending,NotificationAudience.Ops)
);

        return createdRequest;
    }

    public async Task<EventRequestResponseDto> GetByIdForAdminAsync(int id, int adminId)
    {
<<<<<<< HEAD
        var request = await _eventRequestRepository.GetEventRequestByIdAsync(id);

        if (request == null)
            throw new NotFoundException(StringConstant.NoRequestFound);

        if (request.AdminId != adminId)
            throw new ForbiddenException(StringConstant.NoRequestAccess);

        return _mapper.Map<EventRequestResponseDto>(request);
    }

    public async Task<IEnumerable<EventRequestResponseDto>> GetAllEventRequestsAsync(EventRequestFilterDto filter)
    {
        var requests = await _eventRequestRepository.GetEventRequestsByFilterAsync(filter);
        return _mapper.Map<List<EventRequestResponseDto>>(requests);
=======
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
>>>>>>> 3589b78 (Resolve the PR comments and only the specific file for review)
    }

    public async Task<EventRequestResponseDto> EditEventRequestAsync(int id, EditEventRequestDto dto, int adminId)
    {
<<<<<<< HEAD
        var request = await GetOwnedPendingRequestOrThrowAsync(id, adminId, StringConstant.OnlyEditOwnRequest);
=======
        var request = await GetOwnedPendingRequestOrThrowAsync(id, adminId, StringConstant.onlyEditOwnRequest);
>>>>>>> 3589b78 (Resolve the PR comments and only the specific file for review)

        _mapper.Map(dto, request);
        request.UpdatedDate = DateTime.UtcNow;

        await _eventRequestRepository.UpdateAsync(request);
        await _eventRequestRepository.SaveChangesAsync();

        return _mapper.Map<EventRequestResponseDto>(request);
    }

    public async Task<EventRequestResponseDto> WithdrawEventRequestAsync(int id, int adminId)
    {
<<<<<<< HEAD
        var request = await GetOwnedPendingRequestOrThrowAsync(id, adminId, StringConstant.OnlyWithdrawOwnRequest);
=======
        var request = await GetOwnedPendingRequestOrThrowAsync(id, adminId, StringConstant.onlyWithdrawOwnRequest);
>>>>>>> 3589b78 (Resolve the PR comments and only the specific file for review)

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
<<<<<<< HEAD
            throw new NotFoundException(StringConstant.NoRequestFound);
=======
            throw new NotFoundException(StringConstant.noRequestFound);
>>>>>>> 3589b78 (Resolve the PR comments and only the specific file for review)

        if (request.AdminId != adminId)
            throw new ForbiddenException(forbiddenMessage);

        if (request.Status != RequestStatus.Pending)
<<<<<<< HEAD
            throw new ConflictException(StringConstant.EventRequestModifyNotAllowed);

        return request;
    }
=======
            throw new ConflictException(StringConstant.eventRequestModifyNotAllowed);

        return request;
    }

>>>>>>> 3589b78 (Resolve the PR comments and only the specific file for review)
}