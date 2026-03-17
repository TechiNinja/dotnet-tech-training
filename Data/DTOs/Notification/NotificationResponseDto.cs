using SportsManagementApp.Enums;
namespace SportsManagementApp.Data.DTOs;

public class NotificationResponseDto
{
    public int Id { get; set; }
    public int? UserId { get; set; }
    public string Message { get; set; } = string.Empty;
    public NotificationAudience Audience { get; set; }
    public NotificationType Type { get; set; }
    public int EventRequestId { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
}