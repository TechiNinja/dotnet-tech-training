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
        [ProducesResponseType(typeof(FixtureResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetMatchById(int id)
        {
            var result = await _categoryService.GetMatchByIdAsync(id);
            return Ok(result);
        }

        [HttpPatch("{id:int}/sets")]
        [ProducesResponseType(typeof(MatchSetResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> UpdateSet(int id, [FromBody] MatchSetRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _matchService.UpdateSetAsync(id, request);
            return Ok(result);
        }

        [HttpGet("{id:int}/sets")]
        [ProducesResponseType(typeof(IEnumerable<MatchSetResponse>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetSets(int id)
        {
            var result = await _matchService.GetSetsAsync(id);
            return Ok(result);
        }

        [HttpPost("{id:int}/result")]
        [ProducesResponseType(typeof(MatchResultResponse), 201)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> SubmitResult(int id)
        {
            var result = await _matchService.SubmitResultAsync(id);
            return StatusCode(201, result);
        }

        [HttpGet("{id:int}/result")]
        [ProducesResponseType(typeof(MatchResultResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetResult(int id)
        {
            var result = await _matchService.GetResultAsync(id);
            return Ok(result);
        }
    }
}