using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsManagementApp.Data.Filters;
using SportsManagementApp.DTOs.EventCreation;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/event-management")]
    public class EventCreateController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventCreateController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] EventFilterDto filter)
            => Ok(await _eventService.GetAllAsync(filter));

        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _eventService.CreateEventFromRequestAsync(request);
            return CreatedAtAction(nameof(GetAll), new { eventId = result.Id }, result);
        }

        [HttpPatch("{eventId:int}")]
        public async Task<IActionResult> PatchEvent(int eventId, [FromBody] PatchEventRequestDto request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            return Ok(await _eventService.PatchEventAsync(eventId, request));
        }

        [HttpGet("request/{requestId:int}")]
        public async Task<IActionResult> GetEventRequest(int requestId)
            => Ok(await _eventService.GetEventRequestForPreFillAsync(requestId));

        [HttpPatch("{eventId:int}/organizer")]
        public async Task<IActionResult> AssignOrganizer(int eventId, [FromBody] int organizerId)
            => Ok(await _eventService.AssignOrganizerAsync(eventId, organizerId));
    }
}