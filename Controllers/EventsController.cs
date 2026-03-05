using Microsoft.AspNetCore.Mvc;
using SportsManagementApp.DTOs.Request;
using SportsManagementApp.DTOs.Response;
using SportsManagementApp.Services.Interfaces;
using SportsManagementApp.StringConstants;

namespace SportsManagementApp.Controllers
{
    [ApiController]
    [Route("api/events")]
    [Produces("application/json")]
    [Tags(AppConstants.TagEvents)]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService) =>
            _eventService = eventService;

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EventResponse>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAll(
            [FromQuery] int?    id,
            [FromQuery] string? name,
            [FromQuery] string? status,
            [FromQuery] int?    sportId)
        {
            var filter = new EventFilterRequest { Id = id, Name = name, Status = status, SportId = sportId };
            return Ok(await _eventService.GetAllAsync(filter));
        }

        [HttpPost]
        [ProducesResponseType(typeof(EventResponse), 201)]
        [ProducesResponseType(400)][ProducesResponseType(404)]
        [ProducesResponseType(409)][ProducesResponseType(422)]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _eventService.CreateEventFromRequestAsync(request);
            return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
        }

        [HttpGet("request/{requestId:int}")]
        [ProducesResponseType(typeof(EventRequestPreFillResponse), 200)]
        [ProducesResponseType(404)][ProducesResponseType(422)]
        public async Task<IActionResult> GetEventRequest(int requestId) =>
            Ok(await _eventService.GetEventRequestForPreFillAsync(requestId));

        [HttpPatch("{id:int}/organizer")]
        [ProducesResponseType(typeof(EventResponse), 200)]
        [ProducesResponseType(400)][ProducesResponseType(404)][ProducesResponseType(422)]
        public async Task<IActionResult> AssignOrganizer(int id, [FromBody] AssignOrganizerRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(await _eventService.AssignOrganizerAsync(id, request));
        }

        [HttpGet("{id:int}/categories")]
        [ProducesResponseType(typeof(IEnumerable<EventCategoryResponse>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCategories(int id) =>
            Ok(await _eventService.GetCategoriesByEventIdAsync(id));
    }
}
