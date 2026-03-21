using AutoMapper;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Enums;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;
using SportsManagementApp.StringConstants;
using SportsManagementApp.Helper;

namespace SportsManagementApp.Services.Implementations;

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

    public async Task<EventRequestResponseDto> ReviewEventRequestAsync(
    int requestId,
    ReviewEventRequestDto dto,
    int opsUserId)
    {
        if (dto.Status != RequestStatus.Approved && dto.Status != RequestStatus.Rejected)
            throw new ValidationException(StringConstant.OnlyApproveOrRejectAllowed);

        var request = await _eventRequestRepository.GetEventRequestByIdAsync(requestId);
        if (request == null)
            throw new NotFoundException(StringConstant.NoEventFound);

        if (request.Status != RequestStatus.Pending)
            throw new ConflictException(StringConstant.RequestProcessNotAllowed);

        request.Status = dto.Status;
        request.Remarks = dto.Remarks?.Trim() ?? string.Empty;
        request.OperationsReviewerId = opsUserId;
        request.UpdatedDate = DateTime.UtcNow;

        await _operationsRepository.UpdateAsync(request);
        await _operationsRepository.SaveChangesAsync();

        var message = dto.Status == RequestStatus.Approved
            ? $"Your request #{request.Id} has been approved."
            : $"Your request #{request.Id} has been rejected. Remarks: {request.Remarks}";

        await _notificationService.CreateAsync(
            request.CreateNotification(message, dto.Status, NotificationAudience.Admin)
        );

        return _mapper.Map<EventRequestResponseDto>(request);
    }
}