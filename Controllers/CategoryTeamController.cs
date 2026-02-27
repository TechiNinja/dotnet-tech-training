using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SportsManagementApp.Data.DTOs.TeamManagement;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryTeamController : ControllerBase
    {
        private readonly ICategoryTeamService _service;

        public CategoryTeamController(ICategoryTeamService service)
        {
            _service = service;
        }

        [HttpPost("create")]
        public async Task<ActionResult<TeamResponseDto>> CreateTeams(CreateTeamRequestDto request)
        {
            var result = await _service.CreateTeamsAsync(request);
            return Ok(result);
        }

        [HttpGet("{categoryId}")]
        public async Task<ActionResult<List<TeamResponseDto>>> GetTeams(int categoryId)
        {
            var result = await _service.GetTeamsByCategoryAsync(categoryId);
            return Ok(result);
        }
    }
}