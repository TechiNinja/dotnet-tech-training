using Microsoft.AspNetCore.SignalR;
using SportsManagementApp.Common.Exceptions;
using SportsManagementApp.Entities;
using SportsManagementApp.Enums;
using SportsManagementApp.Hubs;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;

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
            throw new ValidationAppException("Invalid requestId.");

        if (string.IsNullOrWhiteSpace(message))
            throw new ValidationAppException("Message is required.");

        if (audience == NotificationAudience.Admin && (!userId.HasValue || userId.Value <= 0))
            throw new ValidationAppException("UserId is required for Admin notifications.");

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

        const string clientEventName = "notification:new";

        if (audience == NotificationAudience.Ops)
        {
            await _hubContext.Clients.Group("ops")
                .SendAsync(clientEventName, payload);
        }
        else
        {
            await _hubContext.Clients.Group($"admin:{userId!.Value}")
                .SendAsync(clientEventName, payload);
        }

        return notification;
    }

    public Task<List<Notification>> GetOpsAsync()
        => _notificationRepository.GetOpsAsync();

    public async Task<List<Notification>> GetAdminAsync(int adminId)
    {
        if (adminId <= 0)
            throw new ValidationAppException("Invalid adminId.");

        return await _notificationRepository.GetAdminAsync(adminId);
    }
}