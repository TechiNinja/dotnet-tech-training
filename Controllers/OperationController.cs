using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Helper;
using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Enums;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Operations")]
public class OperationController : ControllerBase
{
    private readonly IOperationsService _operationsService;

    public OperationController(IOperationsService operationsService)
    {
        _operationsService = operationsService;
    }

    [HttpPut("{id:int}/{status}")]
    [ProducesResponseType(typeof(EventRequestResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<EventRequestResponseDto>> DecideEventRequest(
        int id,
        [FromBody] DecideEventRequestDto dto,
        [FromRoute] RequestStatus status)
    {
        var opsId = User.GetUserId();

        var updated = await _operationsService.DecideAsync(id, dto, opsId, status);
        return Ok(updated);
    }
}