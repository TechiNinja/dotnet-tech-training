using Microsoft.AspNetCore.Mvc;
using SportsManagementApp.DTOs.Response;
using SportsManagementApp.DTOs.Request;
using SportsManagementApp.Services.Interfaces;
using SportsManagementApp.StringConstants;

namespace SportsManagementApp.Controllers
{
    [ApiController]
    [Route("api/v1/fixtures")]
    [Produces("application/json")]
    [Tags(AppConstants.TagFixtures)]
    public class FixturesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public FixturesController(ICategoryService categoryService) =>
            _categoryService = categoryService;

        [HttpGet("{fixtureId:int}")]
        [ProducesResponseType(typeof(FixtureResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetFixtureById(int fixtureId)
        {
            var result = await _categoryService.GetFixtureByIdAsync(fixtureId);
            return Ok(result);
        }

        [HttpPatch("{fixtureId:int}/schedule")]
        [ProducesResponseType(typeof(FixtureResponse), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        public async Task<IActionResult> UpdateSchedule(int fixtureId, [FromBody] ScheduleUpdateRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _categoryService.UpdateScheduleAsync(fixtureId, request);
            return Ok(result);
        }
    }
}