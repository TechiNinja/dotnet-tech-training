using Microsoft.AspNetCore.Mvc;
using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Enums;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EventRequestsController : ControllerBase
{
    private readonly IEventRequestService _eventRequestService;
    public EventRequestsController(IEventRequestService eventRequestService)
    {
        _eventRequestService = eventRequestService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(EventRequestResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EventRequestResponseDto>> RaiseEventRequest(CreateEventRequestDto dto)
    {
        try
        {
            int adminId = 1;
            var result = await _eventRequestService.RaiseEventRequest(dto, adminId);

            return CreatedAtAction(nameof(RaiseEventRequest), new { id = result.Id }, result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.InnerException?.Message ?? ex.Message);
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EventRequestResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<EventRequestResponseDto>>> GetEventSearch(
[FromQuery] int? id,
[FromQuery] RequestStatus? status)
    {
        try
        {
            var result = await _eventRequestService.SearchEventRequests(id, status);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.InnerException?.Message ?? ex.Message);
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(EventRequestResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EventRequestResponseDto>> EditEventRequest(int id, EditEventRequestDto dto)
    {
        try
        {
            var result = await _eventRequestService.EditEventRequest(id, dto);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.InnerException?.Message ?? ex.Message);
        }
    }

    [HttpPatch("{id:int}/withdraw")]
    [ProducesResponseType(typeof(EventRequestResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventRequestResponseDto>> WithdrawEventRequest(int id)
    {
        try
        {
            var result = await _eventRequestService.WithdrawEventRequest(id);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.InnerException?.Message ?? ex.Message);
        }
    }
}


