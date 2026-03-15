using Microsoft.AspNetCore.SignalR;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Enums;
using SportsManagementApp.Hubs;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;
using SportsManagementApp.Constants;

namespace SportsManagementApp.Services.Implementations;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationService(
        INotificationRepository notificationRepository,
        IHubContext<NotificationHub> hubContext)
    {
        _notificationRepository = notificationRepository;
        _hubContext = hubContext;
    }

    public async Task<Notification> CreateAsync(
        int? userId,
        int requestId,
        string message,
        NotificationType type,
        NotificationAudience audience)
    {
        if (requestId <= 0)
            throw new ValidationException(StringConstant.InvalidId);

        if (string.IsNullOrWhiteSpace(message))
            throw new ValidationException(StringConstant.MessageRequired);

        if (audience == NotificationAudience.Admin && (!userId.HasValue || userId.Value <= 0))
            throw new ValidationException(StringConstant.IdRequired);

        if (audience == NotificationAudience.Ops)
            userId = null;

        var notification = new Notification
        {
            UserId = userId,
            Audience = audience,
            EventRequestId = requestId,
            Message = message.Trim(),
            Type = type,
            CreatedAt = DateTime.UtcNow
        };

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

        if (audience == NotificationAudience.Ops)
        {
            await _hubContext.Clients.Group("ops")
                .SendAsync(StringConstant.NewNotificationEvent, payload);
        }
        else
        {
            await _hubContext.Clients.Group($"admin:{userId!.Value}")
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