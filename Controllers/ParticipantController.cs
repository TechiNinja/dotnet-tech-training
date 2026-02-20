using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ParticipantController : ControllerBase
    {
        private readonly IParticipantService _participantService;

        public ParticipantController(IParticipantService participantService)
        {
            _participantService = participantService;
        }

        [HttpGet("{userId}/events")]
        public async Task<IActionResult> GetMyEvents(int userId)
        {
            var events = await _participantService.GetMyEventsAsync(userId);

            return Ok(events);
        }

        [HttpGet("{userId}/team")]
        public async Task<IActionResult> GetMyTeam(int userId)
        {
            var teams = await _participantService.GetMyTeamsAsync(userId);

            return Ok(teams);
        }

        [HttpGet("{userId}/schedule")]
        public async Task<IActionResult> GetMySchedule(int userId)
        {
            var schedule = await _participantService.GetMySchedulesAsync(userId);

            return Ok(schedule);
        }
    }
}
