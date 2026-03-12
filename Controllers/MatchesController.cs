using Microsoft.AspNetCore.Mvc;
using SportsManagementApp.DTOs.Request;
using SportsManagementApp.DTOs.Response;
using SportsManagementApp.Services.Interfaces;
using SportsManagementApp.StringConstants;

namespace SportsManagementApp.Controllers
{
    [ApiController]
    [Route("api/matches")]
    [Produces("application/json")]
    [Tags(AppConstants.TagMatches)]
    public class MatchesController : ControllerBase
    {
        private readonly IMatchService    _matchService;
        private readonly ICategoryService _categoryService;

        public MatchesController(IMatchService matchService, ICategoryService categoryService)
        {
            _matchService    = matchService;
            _categoryService = categoryService;
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetMatchById(int id) =>
            Ok(await _categoryService.GetMatchByIdAsync(id));

        [HttpPatch("{id:int}/sets")]
        public async Task<IActionResult> UpdateSetScore(int id, [FromBody] MatchSetRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(await _matchService.UpdateSetAsync(id, request));
        }

        [HttpGet("{id:int}/sets")]
        public async Task<IActionResult> GetAllSets(int id) =>
            Ok(await _matchService.GetSetsAsync(id));
    }
}