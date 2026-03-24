using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsManagementApp.Enums;
using SportsManagementApp.Helper;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetNotification(
        [FromQuery] NotificationAudience audience,
        [FromQuery] bool? isRead)
    {
        if (audience == NotificationAudience.Ops)
        {
            var data = await _notificationService.GetOpsNotificationAsync(isRead);
            return Ok(data);
        }

        var adminId = User.GetUserId();
        var adminData = await _notificationService.GetAdminNotificationAsync(adminId, isRead);
        return Ok(adminData);
    }

    [HttpGet("unread-count")]
    public async Task<IActionResult> GetUnreadCount([FromQuery] NotificationAudience audience)
    {
        if (audience == NotificationAudience.Ops)
        {
            var count = await _notificationService.GetUnreadCountForOpsAsync();
            return Ok(count);
        }

        var adminId = User.GetUserId();
        var countForAdmin = await _notificationService.GetUnreadCountForAdminAsync(adminId);
        return Ok(countForAdmin);
    }

    [HttpPut("mark-read")]
    public async Task<IActionResult> MarkAsRead([FromQuery] NotificationAudience audience)
    {
        if (audience == NotificationAudience.Ops)
        {
            await _notificationService.MarkOpsAsReadAsync();
            return NoContent();
        }

        var adminId = User.GetUserId();
        await _notificationService.MarkAdminAsReadAsync(adminId);
        return NoContent();
    }
}