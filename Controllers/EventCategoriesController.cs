using Microsoft.AspNetCore.Mvc;
using SportsManagementApp.DTOs.Request;
using SportsManagementApp.DTOs.Response;
using SportsManagementApp.Services.Interfaces;
using SportsManagementApp.StringConstants;

namespace SportsManagementApp.Controllers
{
    [ApiController]
    [Route("api/categories")]
    [Produces("application/json")]
    [Tags(AppConstants.TagCategories)]
    public class EventCategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IFixtureService  _fixtureService;

        public EventCategoriesController(
            ICategoryService categoryService,
            IFixtureService  fixtureService)
        {
            _categoryService = categoryService;
            _fixtureService  = fixtureService;
        }

        [HttpGet("{categoryId:int}")]
        public async Task<IActionResult> GetCategory(int categoryId) =>
            Ok(await _categoryService.GetByIdAsync(categoryId));

        [HttpPost("{categoryId:int}/generate-fixture")]
        public async Task<IActionResult> GenerateFixtures(int categoryId) =>
            StatusCode(201, await _fixtureService.GenerateFixturesAsync(categoryId));

        [HttpGet("{categoryId:int}/fixtures")]
        public async Task<IActionResult> GetFixtures(int categoryId, [FromQuery] string? status = null) =>
            Ok(await _fixtureService.GetFixturesAsync(categoryId, status));

        [HttpDelete("{categoryId:int}/fixtures")]
        public async Task<IActionResult> DeleteFixtures(int categoryId)
        {
            await _fixtureService.DeleteFixturesAsync(categoryId);
            return Ok(new { message = AppConstants.FixturesDeleted });
        }

        [HttpPatch("{categoryId:int}/fixtures/schedule")]
        public async Task<IActionResult> BulkSchedule(int categoryId, [FromBody] List<MatchScheduleItem> schedules)
        {
            return Ok(await _fixtureService.BulkScheduleAsync(categoryId, schedules));
        }
    }
}
