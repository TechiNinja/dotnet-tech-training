using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsManagementApp.Constants;
using SportsManagementApp.Services.Interfaces;
using System.Security.Claims;

namespace SportsManagementApp.Controllers
{
    [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.Organizer},{RoleConstants.Operation}")]
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAnalytics()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var role = User.FindFirstValue(ClaimTypes.Role)!;

            var result = await _analyticsService.GetAnalyticsAsync(userId, role);
            return Ok(result);
        }
    }
}
