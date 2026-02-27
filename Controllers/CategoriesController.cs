using Microsoft.AspNetCore.Mvc;
using SportsManagementApp.DTOs.Request;
using SportsManagementApp.DTOs.Response;
using SportsManagementApp.Services.Interfaces;
using SportsManagementApp.StringConstants;

namespace SportsManagementApp.Controllers
{
    [ApiController]
    [Route("api/v1/categories")]
    [Produces("application/json")]
    [Tags(AppConstants.TagCategories)]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService) =>
            _categoryService = categoryService;

        [HttpGet("{catId:int}")]
        [ProducesResponseType(typeof(CategoryResponse), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCategory(int catId)
        {
            var result = await _categoryService.GetByIdAsync(catId);
            return Ok(result);
        }


        [HttpPost("{catId:int}/fixtures/generate")]
        [ProducesResponseType(typeof(IEnumerable<FixtureResponse>), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> GenerateFixtures(int catId)
        {
            var result = await _categoryService.GenerateFixturesAsync(catId);
            return StatusCode(201, result);
        }

        [HttpGet("{catId:int}/fixtures")]
        [ProducesResponseType(typeof(IEnumerable<FixtureResponse>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetFixtures(int catId, [FromQuery] string? status = null)
        {
            var result = await _categoryService.GetFixturesAsync(catId, status);
            return Ok(result);
        }


        [HttpDelete("{catId:int}/fixtures")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteFixtures(int catId)
        {
            await _categoryService.DeleteFixturesAsync(catId);
            return Ok(new { message = AppConstants.FixturesDeleted });
        }

        [HttpPost("{catId:int}/fixtures/publish")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        public async Task<IActionResult> PublishSchedule(int catId)
        {
            await _categoryService.PublishScheduleAsync(catId);
            return Ok(new { message = AppConstants.SchedulePublished });
        }

    }
}