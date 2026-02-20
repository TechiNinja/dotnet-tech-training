using Microsoft.AspNetCore.Mvc;
using SportsManagementApp.DTOs;
using SportsManagementApp.Enums;
using SportsManagementApp.Entities;
using SportsManagementApp.Services;
using SportsManagementApp.Helper;

namespace SportsManagementApp.Controllers
{
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
        [ProducesResponseType(typeof(ApiResponseSuccess<EventRequestResponseDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponseError), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EventRequestResponseDto>> RaiseEventRequest(CreateEventRequestDto dto)
        {
            try
            {
                int adminId = 1;
                var result = await _eventRequestService.RaiseEventRequest(dto, adminId);

                var response = new ApiResponseSuccess<EventRequestResponseDto>
                {
                    Message = StringConstant.eventCreated,
                    Data = result
                };


                return CreatedAtAction(nameof(RaiseEventRequest), new { id = result.Id }, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseError
                {
                    Message = ex.InnerException?.Message ?? ex.Message
                });
            }
        }



        [HttpGet]
        [ProducesResponseType(typeof(ApiResponseSuccess<IEnumerable<EventRequestResponseDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseError), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<EventRequestResponseDto>>> GetEventSearch(
    [FromQuery] int? id,
    [FromQuery] RequestStatus? status)
        {
            try
            {
                var result = await _eventRequestService.SearchEventRequests(id, status);

                return Ok(new ApiResponseSuccess<IEnumerable<EventRequestResponseDto>>
                {
                    Message = result.Any() ? StringConstant.eventRequestSuccess : StringConstant.noEventFound,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseError { Message = ex.Message });
            }
        }


        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponseSuccess<EventRequestResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseError), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EventRequestResponseDto>> EditEventRequest(int id, EditEventRequestDto dto)
        {
            try
            {
                var result = await _eventRequestService.EditEventRequest(id, dto);

                if (result == null)
                {
                    return NotFound(new ApiResponseError
                    {
                        Message = StringConstant.noEventFound,
                    });
                }

                return Ok(new ApiResponseSuccess<EventRequestResponseDto>
                {
                    Message = StringConstant.eventUpdated,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseError
                {
                    Message = ex.Message
                });
            }
        }

        [HttpPatch("{id:int}/withdraw")]
        [ProducesResponseType(typeof(ApiResponseSuccess<EventRequestResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseError), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EventRequestResponseDto>> WithdrawEventRequest(int id)
        {
            try
            {
                var result = await _eventRequestService.WithdrawlEventRequest(id);

                if (result == null)
                {
                    return NotFound(new ApiResponseError
                    {
                        Message = StringConstant.noRequestFound,
                    });
                }

                return Ok(new ApiResponseSuccess<EventRequestResponseDto>
                {
                    Message = StringConstant.eventRequestWithdrawl,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseError
                {
                    Message = ex.Message
                });
            }
        }
    }
}
