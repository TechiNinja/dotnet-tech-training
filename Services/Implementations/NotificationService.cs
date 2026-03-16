using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using SportsManagementApp.StringConstants;
using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Enums;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Hubs;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Services.Implementations;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly IMapper _mapper;

    public NotificationService(
        INotificationRepository notificationRepository,
        IHubContext<NotificationHub> hubContext,
        IMapper mapper)
    {
        _notificationRepository = notificationRepository;
        _hubContext = hubContext;
        _mapper = mapper;
    }

    public async Task<Notification> CreateAsync(CreateNotificationDto dto)
    {
        if (dto.EventRequestId <= 0)
            throw new ValidationException(StringConstant.InvalidId);

        if (string.IsNullOrWhiteSpace(dto.Message))
            throw new ValidationException(StringConstant.MessageRequired);

        if (dto.Audience == NotificationAudience.Admin &&
            (!dto.UserId.HasValue || dto.UserId.Value <= 0))
            throw new ValidationException(StringConstant.IdRequired);

        if (dto.Audience == NotificationAudience.Ops)
            dto.UserId = null;

        dto.Message = dto.Message.Trim();

        var notification = _mapper.Map<Notification>(dto);

        await _notificationRepository.AddAsync(notification);
        await _notificationRepository.SaveChangesAsync();

        var payload = new
        {
            id = notification.Id,
            userId = notification.UserId,
            audience = notification.Audience.ToString(),
            eventRequestId = notification.EventRequestId,
            message = notification.Message,
            type = notification.Type.ToString(),
            createdAt = notification.CreatedAt
        };

        if (notification.Audience == NotificationAudience.Ops)
        {
            await _hubContext.Clients.Group("ops")
                .SendAsync(StringConstant.NewNotificationEvent, payload);
        }
        else
        {
            await _hubContext.Clients.Group($"admin:{notification.UserId!.Value}")
                .SendAsync(StringConstant.NewNotificationEvent, payload);
        }

        return notification;
    }

    public async Task<List<Notification>> GetOpsAsync()
    {
        return await _notificationRepository.GetOpsAsync();
    }

    public async Task<List<Notification>> GetAdminAsync(int adminId)
    {
        if (adminId <= 0)
            throw new ValidationException(StringConstant.InvalidId);

        return await _notificationRepository.GetAdminAsync(adminId);
    }
}