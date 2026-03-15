using AutoMapper;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Enums;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;
using SportsManagementApp.Constants;
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
        DecideEventRequestDto dto,
        int opsUserId,
        RequestStatus status)
    {
        if (status != RequestStatus.Approved && status != RequestStatus.Rejected)
            throw new ValidationException(StringConstant.onlyApproveorRejectAllowed);

        var request = await _eventRequestRepository.GetEventRequestByIdAsync(requestId);
        if (request == null)
            throw new NotFoundException(StringConstant.noEventFound);

        if (request.Status != RequestStatus.Pending)
            throw new ConflictException(StringConstant.requestProcessNotAllowed);

        request.Status = status;
        request.Remarks = dto.Remarks.Trim();
        request.OperationsReviewerId = opsUserId;
        request.UpdatedDate = DateTime.UtcNow;

        await _operationsRepository.UpdateAsync(request);
        await _operationsRepository.SaveChangesAsync();

        var message = status == RequestStatus.Approved
            ? $"Your request #{request.Id} has been approved."
            : $"Your request #{request.Id} has been rejected. Remarks: {request.Remarks}";
            
                await _notificationService.CreateAsync(
    request.CreateNotification(message, status)
);

        return _mapper.Map<EventRequestResponseDto>(request);
    }
}