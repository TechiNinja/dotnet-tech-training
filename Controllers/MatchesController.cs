using Microsoft.AspNetCore.Mvc;
using SportsManagementApp.DTOs.Request;
using SportsManagementApp.DTOs.Response;
using SportsManagementApp.Services.Interfaces;
using SportsManagementApp.StringConstants;

namespace SportsManagementApp.Controllers
{
    [ApiController]
    [Route("api/v1/matches")]
    [Produces("application/json")]
    [Tags(AppConstants.TagMatches)]
    public class MatchesController : ControllerBase
    {
        private readonly IMatchService _matchService;

        public MatchesController(IMatchService matchService) =>
            _matchService = matchService;

        [HttpPost("{id:int}/sets")]
        [ProducesResponseType(typeof(MatchSetResponse), 201)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> StartSet(int id)
        {
            var result = await _matchService.StartSetAsync(id);
            return StatusCode(201, result);
        }

        [HttpPatch("{id:int}/sets/{setId:int}")]
        [ProducesResponseType(typeof(MatchSetResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> UpdateSetScore(
            int id, int setId, [FromBody] MatchSetRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _matchService.UpdateSetScoreAsync(id, setId, request);
            return Ok(result);
        }

        [HttpPut("{id:int}/sets/{setId:int}/complete")]
        [ProducesResponseType(typeof(MatchSetResponse), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> CompleteSet(int id, int setId)
        {
            var result = await _matchService.CompleteSetAsync(id, setId);
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