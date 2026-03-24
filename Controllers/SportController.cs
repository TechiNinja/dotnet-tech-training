using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportsManagementApp.Constants;
using SportsManagementApp.Data.DTOs.SportManagement;
using SportsManagementApp.Data.Filters;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Controllers
{
    [Authorize]
    [Route("api/sports")]
    [ApiController]
    public class SportController : ControllerBase
    {
        private readonly ISportService _sportsService;

        public SportController(ISportService sportsService)
        {
            _sportsService = sportsService;
        }

        [Authorize(Roles = RoleConstants.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateSport([FromBody] CreateSportDto createSport)
        {
            var sport = await _sportsService.CreateSportAsync(createSport);
            return Ok(sport);
        }

        [HttpGet]
        public async Task<IActionResult> GetSports([FromQuery] SportFilterDto filter)
        {
            var sports = await _sportsService.GetSportsAsync(filter);
            return Ok(sports);
        }

        [Authorize(Roles = RoleConstants.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSport(int id, [FromBody] UpdateSportDto updateSport)
        {
            var sport = await _sportsService.UpdateSportAsync(id, updateSport);
            return Ok(sport);
        }
    }
}