using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsManagementApp.Common.Exceptions;
using SportsManagementApp.Common.Helper;
using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Data.Filters;
using SportsManagementApp.Enums;
using SportsManagementApp.Extensions;
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


    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ProducesResponseType(typeof(EventRequestResponseDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<EventRequestResponseDto>> RaiseEventRequest([FromBody] CreateEventRequestDto dto)
    {
        var adminId = User.GetUserId();
        var created = await _eventRequestService.RaiseEventRequestAsync(dto, adminId);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EventRequestResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<EventRequestResponseDto>> GetById(int id)
    {
        var adminId = User.GetUserId();
        var result = await _eventRequestService.GetByIdForAdminAsync(id, adminId);

        return Ok(result);
    }

    [Authorize(Roles = "Admin,Operations")]
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<EventRequestResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<EventRequestResponseDto>>> Search(
    [FromQuery] EventRequestFilterDto filter)
    {
        var result = await _eventRequestService.SearchEventRequestsAsync(filter);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(EventRequestResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<EventRequestResponseDto>> EditEventRequest(int id, [FromBody] EditEventRequestDto dto)
    {
        var adminId = User.GetUserId();
        var updated = await _eventRequestService.EditEventRequestAsync(id, dto, adminId);

        return Ok(updated);
    }

    [Authorize(Roles = "Admin")]
    [HttpPatch("{id:int}/withdraw")]
    [ProducesResponseType(typeof(EventRequestResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<EventRequestResponseDto>> WithdrawEventRequest(int id)
    {
        var adminId = User.GetUserId();
        var updated = await _eventRequestService.WithdrawEventRequestAsync(id, adminId);

        return Ok(updated);
    }
}