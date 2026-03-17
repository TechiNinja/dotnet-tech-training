using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Helper;
using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Enums;
using SportsManagementApp.Services.Interfaces;
using SportsManagementApp.Constants;

namespace SportsManagementApp.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = $"{RoleConstants.Operation}")]
public class OperationController : ControllerBase
{
    private readonly IOperationsService _operationsService;

    public OperationController(IOperationsService operationsService)
    {
        _operationsService = operationsService;
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(EventRequestResponseDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<EventRequestResponseDto>> ReviewEventRequest(
        int id,
        [FromBody] ReviewEventRequestDto dto)
    {
        var opsId = User.GetUserId();
        var updated = await _operationsService.ReviewEventRequestAsync(id, dto, opsId);
        return Ok(updated);
    }
}