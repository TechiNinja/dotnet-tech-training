using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportsManagementApp.Constants;
using SportsManagementApp.Data.DTOs.SportManagement;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
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
            try
            {
                var sport = await _sportsService.CreateSportAsync(createSport);
                return Ok(sport);
            }
            catch (Exception)
            {
                return Problem("An error occurred whlie creating sport");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetSports()
        {
            var sports = await _sportsService.GetSportsAsync();
            return Ok(sports);
        }

        [Authorize(Roles = RoleConstants.Admin)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSport(int id, [FromBody] UpdateSportDto updateSport)
        {
            try
            {
                var sport = await _sportsService.UpdateSportAsync(id, updateSport);
                return Ok(sport);
            }
            catch (Exception)
            {
                return Problem("An error occurred while updating sport");
            }
        }
    }
}
