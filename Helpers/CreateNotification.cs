using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Enums;

namespace SportsManagementApp.Helper;

public static class NotificationHelper
{
    public static BaseNotificationDto CreateNotification(
        this EventRequest request,
        string message,
        RequestStatus status,NotificationAudience audience)
    {
        return new BaseNotificationDto
        {
            UserId = audience == NotificationAudience.Admin ? request.AdminId : request.OperationsReviewerId,
            EventRequestId = request.Id,
            Message = message,
            Type = status == RequestStatus.Approved
                ? NotificationType.Approved
                : NotificationType.Rejected,
            Audience = audience,
        };
    }
}