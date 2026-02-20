using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MyTeamsController : ControllerBase
    {
        private readonly IParticipantService _participantService;

        public MyTeamsController(IParticipantService participantService)
        {
            _participantService = participantService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetMyTeam(int userId)
        {
            var teams = await _participantService.GetMyTeamsAsync(userId);

            return Ok(teams);
        }
    }
}
