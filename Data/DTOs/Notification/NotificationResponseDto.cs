namespace SportsManagementApp.Data.DTOs;

public class NotificationResponseDto : BaseNotificationDto
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
}