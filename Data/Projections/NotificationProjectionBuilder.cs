using System.Linq.Expressions;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Data.DTOs;

public static class NotificationProjectionBuilder
{
    public static Expression<Func<Notification, NotificationResponseDto>> ToDto()
    {
        return n => new NotificationResponseDto
        {
            Id = n.Id,
            UserId = n.UserId,
            EventRequestId = n.EventRequestId,
            Message = n.Message,
            Type = n.Type,
            Audience = n.Audience,
            CreatedAt = n.CreatedAt,
            IsRead = n.IsRead
        };
    }
}