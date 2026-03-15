using SportsManagementApp.Enums;
using System.ComponentModel.DataAnnotations;

namespace SportsManagementApp.Data.DTOs;

public class NotificationCreateDto
{
    public int? UserId { get; set; }
    public int RequestId { get; set; }
    public string Message { get; set; }
    public NotificationType Type { get; set; }
    public NotificationAudience Audience { get; set; }
}