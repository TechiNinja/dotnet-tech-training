using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportsManagementApp.Data.DTOs.TeamManagement;
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

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserTeam(int userId)
        {
            var teams = await _teamsService.GetUserTeamsAsync(userId);

            return Ok(teams);
        }

        [HttpPost("create")]
        [Authorize(Roles = "Organizer")]
        public async Task<ActionResult<TeamResponseDto>> CreateTeams(CreateTeamRequestDto request)
        {
            var result = await _teamsService.CreateTeamsAsync(request);
            return Ok(result);
        }

        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<List<TeamResponseDto>>> GetTeams(int categoryId)
        {
            var result = await _teamsService.GetTeamsByCategoryAsync(categoryId);
            return Ok(result);
        }
    }
}
