using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MySchedulesController : ControllerBase
    {
        private readonly IParticipantService _participantService;

        public MySchedulesController(IParticipantService participantService)
        {
            _participantService = participantService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetMySchedule(int userId)
        {
            var schedule = await _participantService.GetMySchedulesAsync(userId);

            return Ok(schedule);
        }
    }
}
