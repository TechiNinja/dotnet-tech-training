using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Helper;
using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Data.Filters;
using SportsManagementApp.Enums;
using SportsManagementApp.Extensions;
using SportsManagementApp.Constants;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Controllers;

[ApiController]
[Route("api/event-requests")]
[Authorize]
public class EventRequestsController : ControllerBase
{
    private readonly IEventRequestService _eventRequestService;

    public EventRequestsController(IEventRequestService eventRequestService)
    {
        _eventRequestService = eventRequestService;
    }
    
    [Authorize(Roles = $"{RoleConstants.Admin}")]
    [HttpPost]
    [ProducesResponseType(typeof(EventRequestResponseDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<EventRequestResponseDto>> RaiseEventRequest([FromBody] CreateEventRequestDto dto)
    {
        var adminId = User.GetUserId();
        var created = await _eventRequestService.RaiseEventRequestAsync(dto, adminId);

        return CreatedAtAction(nameof(GetEventRequestById), new { id = created.Id }, created);
    }

    [Authorize(Roles = $"{RoleConstants.Admin}")]
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EventRequestResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<EventRequestResponseDto>> GetEventRequestById(int id)
    {
        var adminId = User.GetUserId();
        var result = await _eventRequestService.GetByIdForAdminAsync(id, adminId);

        return Ok(result);
    }

    [Authorize(Roles = $"{RoleConstants.Admin},{RoleConstants.Operation}")]
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EventRequestResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EventRequestResponseDto>>> GetEventRequest(
    [FromQuery] EventRequestFilterDto filter)
    {
        var result = await _eventRequestService.GetAllEventRequestsAsync(filter);
        return Ok(result);
    }

    [Authorize(Roles = $"{RoleConstants.Admin}")]
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(EventRequestResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<EventRequestResponseDto>> EditEventRequest(int id, [FromBody] BaseEventRequestDto dto)
    {
        var adminId = User.GetUserId();
        var updated = await _eventRequestService.EditEventRequestAsync(id, dto, adminId);

        return Ok(updated);
    }

    [Authorize(Roles = $"{RoleConstants.Admin}")]
    [HttpPatch("{id:int}/withdraw")]
    [ProducesResponseType(typeof(EventRequestResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<EventRequestResponseDto>> WithdrawEventRequest(int id)
    {
        var adminId = User.GetUserId();
        var updated = await _eventRequestService.WithdrawEventRequestAsync(id, adminId);

        return Ok(updated);
    }
}