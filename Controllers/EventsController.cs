using Microsoft.AspNetCore.Mvc;
using SportsManagementApp.DTOs.Request;
using SportsManagementApp.DTOs.Response;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Controllers
{
    [ApiController]
    [Route("api/events")]
    [Produces("application/json")]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService) =>
            _eventService = eventService;

        [HttpGet]
        [ProducesResponseType(typeof(List<EventResponse>), 200)]
        public async Task<IActionResult> GetAll()
        {
            var result = await _eventService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(EventResponse), 200)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _eventService.GetByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(EventResponse), 201)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 409)]
        [ProducesResponseType(typeof(object), 422)]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _eventService.CreateEventFromRequestAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPatch("{id:int}/organizer")]
        [ProducesResponseType(typeof(EventResponse), 200)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 422)]
        public async Task<IActionResult> AssignOrganizer(int id, [FromBody] AssignOrganizerRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _eventService.AssignOrganizerAsync(id, request);
            return Ok(result);
        }

        [HttpPatch("{id:int}/configuration")]
        [ProducesResponseType(typeof(EventResponse), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 422)]
        public async Task<IActionResult> ConfigureEvent(int id, [FromBody] EventConfigurationRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _eventService.ConfigureEventAsync(id, request);
            return Ok(result);
        }
    }
}