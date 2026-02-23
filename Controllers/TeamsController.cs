using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamsService _teamssService;

        public TeamsController(ITeamsService teamsService)
        {
            _teamssService = teamsService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserTeam(int userId)
        {
            var teams = await _teamssService.GetUserTeamsAsync(userId);

            return Ok(teams);
        }
    }
}
