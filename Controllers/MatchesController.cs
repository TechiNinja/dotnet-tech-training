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
        private readonly IMatchService    _matchService;
        private readonly ICategoryService _categoryService;

        public MatchesController(IMatchService matchService, ICategoryService categoryService)
        {
            _matchService    = matchService;
            _categoryService = categoryService;
        }

        [HttpGet("{matchId:int}")]
        public async Task<IActionResult> GetMatchById(int matchId) =>
            Ok(await _categoryService.GetMatchByIdAsync(matchId));

        [HttpGet("{matchId:int}/sets")]
        public async Task<IActionResult> GetAllSets(int matchId) =>
            Ok(await _matchService.GetSetsAsync(matchId));

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