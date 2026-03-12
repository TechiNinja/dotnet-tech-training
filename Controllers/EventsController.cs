using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsManagementApp.DTOs.Request;
using SportsManagementApp.DTOs.Response;
using SportsManagementApp.Services.Interfaces;
using SportsManagementApp.StringConstants;

namespace SportsManagementApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/events")]
    [Produces("application/json")]
    [Tags(AppConstants.TagEvents)]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IEventsService _eventsService;

        public EventsController(IEventService eventService, IEventsService eventsService)
        {
            _eventService = eventService;
            _eventsService = eventsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? id,
            [FromQuery] string? name,
            [FromQuery] string? status,
            [FromQuery] int? sportId)
        {
            var filter = new EventFilterRequest { Id = id, Name = name, Status = status, SportId = sportId };
            return Ok(await _eventService.GetAllAsync(filter));
        }

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _eventService.CreateEventFromRequestAsync(request);
            return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
        }

        [HttpGet("request/{requestId:int}")]
        public async Task<IActionResult> GetEventRequest(int requestId) =>
            Ok(await _eventService.GetEventRequestForPreFillAsync(requestId));

        [HttpPatch("{id:int}/organizer")]
        public async Task<IActionResult> AssignOrganizer(int id, [FromBody] int organizerId)
        {
            return Ok(await _eventService.AssignOrganizerAsync(id, organizerId));
        }

        [HttpGet("{id:int}/categories")]
        public async Task<IActionResult> GetCategories(int id) =>
            Ok(await _eventService.GetCategoriesByEventIdAsync(id));

        [HttpGet("user/{userId:int}")]
        public async Task<IActionResult> GetUserEvents(int userId)
        {
            var events = await _eventsService.GetUserEventsAsync(userId);
            return Ok(events);
        }
    }
}