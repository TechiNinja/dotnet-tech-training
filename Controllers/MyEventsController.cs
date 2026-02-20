using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MyEventsController : ControllerBase
    {
        private readonly IParticipantService _participantService;

        public MyEventsController(IParticipantService participantService)
        {
            _participantService = participantService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetMyEvents(int userId)
        {
            var events = await _participantService.GetMyEventsAsync(userId);

            return Ok(events);
        }
    }
}
