using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Enums;

namespace SportsManagementApp.Helper;

public static class CreateNotificationHelper
{
    public static CreateNotificationDto CreateNotification(
        this EventRequest request,
        string message,
        RequestStatus status)
    {
        return new CreateNotificationDto
        {
            UserId = request.AdminId,
            EventRequestId = request.Id,
            Message = message,
            Type = status == RequestStatus.Approved
                ? NotificationType.Approved
                : NotificationType.Rejected,
            Audience = NotificationAudience.Admin
        };
    }
}