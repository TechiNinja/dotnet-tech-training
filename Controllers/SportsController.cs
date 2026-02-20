using Microsoft.AspNetCore.Mvc;
using SportsManagementApp.DTOs;
using SportsManagementApp.Entities;
using SportsManagementApp.Helper;
using SportsManagementApp.Services;

namespace SportsManagementApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SportsController : ControllerBase
    {
        private readonly ISportService _sportService;

        public SportsController(ISportService sportService)
        {
            _sportService = sportService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponseSuccess<Sport>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponseError), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Sport>> CreateSports(CreateSportDto dto)
        {
            try
            {
                var result = await _sportService.CreateSport(dto);

                return CreatedAtAction(nameof(CreateSports), new { id = result.Id }, new ApiResponseSuccess<Sport>
                {
                    Message = StringConstant.sportsCreated,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseError
                {
                    Message = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        [HttpGet()]
        [ProducesResponseType(typeof(ApiResponseSuccess<IEnumerable<Sport>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseError), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Sport>>> SearchSports([FromQuery] int? id, [FromQuery] string? name)
        {
            try
            {
                var result = await _sportService.SearchSports(id, name);

                return Ok(new ApiResponseSuccess<IEnumerable<Sport>>
                {
                    Message = result.Any() ? StringConstant.sportsFetchSuccess : StringConstant.sportNotFound,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseError
                {
                    Message = ex.InnerException?.Message ?? ex.Message
                });
            }
        }


    }
}
