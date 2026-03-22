using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsManagementApp.Constants;
using SportsManagementApp.DTOs.Fixture;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/matches")]
    public class MatchesController : ControllerBase
    {
        private readonly IMatchService _matchService;
        private readonly IEventCategoryService _eventCategoryService;
        private readonly IFixtureService _fixtureService;

        public MatchesController(
            IMatchService matchService,
            IEventCategoryService eventCategoryService,
            IFixtureService fixtureService)
        {
            _matchService = matchService;
            _fixtureService = fixtureService;
            _eventCategoryService = eventCategoryService;
        }

        [HttpGet("category/{categoryId:int}/fixtures")]
        public async Task<IActionResult> GetFixtures(int categoryId, [FromQuery] string? status = null) =>
            Ok(await _fixtureService.GetFixturesAsync(categoryId, status));

        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.Organizer}")]
        [HttpDelete("category/{categoryId:int}/fixtures")]
        public async Task<IActionResult> DeleteFixtures(int categoryId)
        {
            await _fixtureService.DeleteFixturesAsync(categoryId);
            return Ok(new { message = StringConstant.FixturesDeleted });
        }

        [HttpGet("{matchId:int}")]
        public async Task<IActionResult> GetMatchById(int matchId) =>
            Ok(await _eventCategoryService.GetMatchByIdAsync(matchId));

        [HttpGet("{matchId:int}/sets")]
        public async Task<IActionResult> GetAllSets(int matchId) =>
            Ok(await _matchService.GetSetsAsync(matchId));

        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.Organizer}")]
        [HttpPatch("{matchId:int}/reschedule")]
        public async Task<IActionResult> Reschedule(int matchId, [FromBody] DateTime newStartDateTime) =>
            Ok(await _matchService.RescheduleAsync(matchId, newStartDateTime));

        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.Organizer}")]
        [HttpPatch("{matchId:int}/sets")]
        public async Task<IActionResult> UpdateSetScore(int matchId, [FromBody] MatchSetRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(await _matchService.UpdateSetAsync(matchId, request));
        }

        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.Organizer}")]
        [HttpPatch("{matchId:int}/sets/{setId:int}")]
        public async Task<IActionResult> UpdateSetById(int matchId, int setId, [FromBody] MatchSetRequestDto request) =>
            Ok(await _matchService.UpdateSetByIdAsync(matchId, setId, request));
    }
}