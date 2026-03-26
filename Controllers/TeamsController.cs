using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsManagementApp.Constants;
using SportsManagementApp.Data.DTOs.TeamManagement;
using SportsManagementApp.Data.Filters;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamsService _teamsService;

        public TeamsController(ITeamsService teamsService)
        {
            _teamsService = teamsService;
        }

        [HttpGet]
        public async Task<ActionResult<List<TeamResponseDto>>> GetTeams([FromQuery] TeamFilterDto filter)
        {
            var teams = await _teamsService.GetTeamsAsync(filter);
            return Ok(teams);
        }

        [HttpPost("create")]
        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.Organizer}")]
        public async Task<ActionResult<List<TeamResponseDto>>> CreateTeams([FromBody] CreateTeamRequestDto request)
        {
            var result = await _teamsService.CreateTeamsAsync(request);
            return Ok(result);
        }
    }
}