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
            return Ok(result.Data!);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(EventResponse), 200)]
        [ProducesResponseType(typeof(object), 404)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _eventService.GetByIdAsync(id);
            return result.IsSuccess
                ? Ok(result.Data!)
                : NotFound(new { message = result.Error! });
        }

        [HttpPost]
        [ProducesResponseType(typeof(EventResponse), 201)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 409)]
        [ProducesResponseType(typeof(object), 422)]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _eventService.CreateEventFromRequestAsync(request);

            return result.StatusCode switch
            {
                201 => CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data!),
                404 => NotFound(new            { message = result.Error! }),
                409 => Conflict(new            { message = result.Error! }),
                422 => UnprocessableEntity(new { message = result.Error! }),
                _   => BadRequest(new          { message = result.Error! })
            };
        }

        [HttpPatch("{id:int}/organizer")]
        [ProducesResponseType(typeof(EventResponse), 200)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 422)]
        public async Task<IActionResult> AssignOrganizer(int id, [FromBody] AssignOrganizerRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _eventService.AssignOrganizerAsync(id, request);

            return result.StatusCode switch
            {
                200 => Ok(result.Data!),
                404 => NotFound(new            { message = result.Error! }),
                422 => UnprocessableEntity(new { message = result.Error! }),
                _   => BadRequest(new          { message = result.Error! })
            };
        }

        [HttpPut("{id:int}/configuration")]
        [ProducesResponseType(typeof(EventResponse), 200)]
        [ProducesResponseType(typeof(object), 400)]
        [ProducesResponseType(typeof(object), 404)]
        [ProducesResponseType(typeof(object), 422)]
        public async Task<IActionResult> ConfigureEvent(int id, [FromBody] EventConfigurationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _eventService.ConfigureEventAsync(id, request);

            return result.StatusCode switch
            {
                200 => Ok(result.Data!),
                404 => NotFound(new            { message = result.Error! }),
                422 => UnprocessableEntity(new { message = result.Error! }),
                _   => BadRequest(new          { message = result.Error! })
            };
        }
    }
}