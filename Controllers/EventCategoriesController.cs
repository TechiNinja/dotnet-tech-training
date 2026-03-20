using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsManagementApp.Constants;
using SportsManagementApp.DTOs.EventCreation;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/categories")]
    public class EventCategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IFixtureService  _fixtureService;

        public EventCategoriesController(ICategoryService categoryService, IFixtureService fixtureService)
        {
            _categoryService = categoryService;
            _fixtureService  = fixtureService;
        }

        [HttpGet("{categoryId:int}")]
        public async Task<IActionResult> GetCategory(int categoryId) =>
            Ok(await _categoryService.GetByIdAsync<EventCategoryResponseDto>(categoryId));

        [HttpGet("{categoryId:int}/fixtures")]
        public async Task<IActionResult> GetFixtures(int categoryId, [FromQuery] string? status = null) =>
            Ok(await _fixtureService.GetFixturesAsync(categoryId, status));

        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.Organizer}")]
        [HttpPost("{categoryId:int}/generate-fixture")]
        public async Task<IActionResult> GenerateFixtures(int categoryId) =>
            StatusCode(201, await _fixtureService.GenerateFixturesAsync(categoryId));

        [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.Organizer}")]
        [HttpDelete("{categoryId:int}/fixtures")]
        public async Task<IActionResult> DeleteFixtures(int categoryId)
        {
            await _fixtureService.DeleteFixturesAsync(categoryId);
            return Ok(new { message = StringConstant.FixturesDeleted });
        }
    }
}