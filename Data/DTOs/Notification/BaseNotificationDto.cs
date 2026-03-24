using SportsManagementApp.Enums;

namespace SportsManagementApp.Data.DTOs;

public class BaseNotificationDto
{
    public int? UserId { get; set; }
    public int EventRequestId { get; set; }
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public NotificationAudience Audience { get; set; }
}