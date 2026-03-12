using AutoMapper;
using SportsManagementApp.Common.Exceptions;
using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Enums;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Services.OperationsService;

public class OperationsService : IOperationsService
{
    private readonly IOperationsRepository _operationsRepository;
    private readonly INotificationService _notificationService;
    private readonly IEventRequestRepository _eventRequestRepository;
    private readonly IMapper _mapper;

    public OperationsService(
        IOperationsRepository operationRepository,
        INotificationService notificationService,
        IEventRequestRepository eventRequestRepository,
        IMapper mapper)
    {
        _operationsRepository = operationRepository;
        _notificationService = notificationService;
        _eventRequestRepository = eventRequestRepository;
        _mapper = mapper;
    }

    public async Task<EventRequestResponseDto> DecideAsync(
        int requestId,
        DecideEventRequestDto dto,
        int opsUserId,
        RequestStatus status)
    {
        if (status != RequestStatus.Approved && status != RequestStatus.Rejected)
            throw new ValidationAppException("Only Approved or Rejected decisions are allowed.");

        var request = await _eventRequestRepository.GetEventRequestByIdAsync(requestId);
        if (request == null)
            throw new NotFoundAppException("Event request not found.");

        if (request.Status != RequestStatus.Pending)
            throw new ConflictAppException("Request already processed. Double approval or rejection is not allowed.");

        request.Status = status;
        request.Remarks = dto.Remarks.Trim();
        request.OperationsReviewerId = opsUserId;
        request.UpdatedDate = DateTime.UtcNow;

        _operationsRepository.Update(request);
        await _operationsRepository.SaveChangesAsync();

        var message = status == RequestStatus.Approved
            ? $"Your request #{request.Id} has been approved."
            : $"Your request #{request.Id} has been rejected. Remarks: {request.Remarks}";

        await _notificationService.CreateAsync(
            userId: request.AdminId,
            requestId: request.Id,
            message: message,
            type: status == RequestStatus.Approved ? NotificationType.Approved : NotificationType.Rejected,
            audience: NotificationAudience.Admin
        );

        return _mapper.Map<EventRequestResponseDto>(request);
    }
}