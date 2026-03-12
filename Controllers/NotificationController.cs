using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsManagementApp.Common.Exceptions;
using SportsManagementApp.Enums;
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
    public async Task<IActionResult> Get([FromQuery] NotificationAudience audience)
    {
        if (audience == NotificationAudience.Ops)
        {
            var data = await _notificationService.GetOpsAsync();
            return Ok(data);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(userId, out var adminId))
            throw new UnauthorizedAppException("UserId not found in token.");

        var adminData = await _notificationService.GetAdminAsync(adminId);
        return Ok(adminData);
    }
}