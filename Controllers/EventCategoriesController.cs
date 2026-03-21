using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsManagementApp.Constants;
using SportsManagementApp.DTOs.EventCreation;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/eventCategories")]
    public class EventCategoriesController : ControllerBase
    {
        private readonly IEventCategoryService _eventCategoryService;
        private readonly IFixtureService  _fixtureService;

        public EventCategoriesController(IEventCategoryService eventCategoryService, IFixtureService fixtureService)
        {
            _eventCategoryService = eventCategoryService;
            _fixtureService  = fixtureService;
        }

        [HttpGet("{categoryId:int}")]
        public async Task<IActionResult> GetCategory(int categoryId) =>
            Ok(await _eventCategoryService.GetByIdAsync<EventCategoryResponseDto>(categoryId));

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