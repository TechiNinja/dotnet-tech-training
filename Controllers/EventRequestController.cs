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
        [ProducesResponseType(typeof(ApiResponseSuccess<EventRequest>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponseError<string>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EventRequest>> RaiseEventRequest(CreateEventRequestDto dto)
        {
            try
            {
                int adminId = 1;
                var result = await _eventRequestService.RaiseEventRequest(dto, adminId);

                var response = new ApiResponseSuccess<EventRequest>
                {
                    Message = StringConstant.eventCreated,
                    Data = result
                };

                return CreatedAtAction(nameof(GetEventRequestById), new { id = result.Id }, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseError<string>
                {
                    Message = ex.InnerException?.Message ?? ex.Message
                });
            }
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponseSuccess<EventRequestResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseError<string>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<EventRequestResponseDto>>> GetAllEventRequest()
        {
            try
            {
                var result = await _eventRequestService.GetAllEventRequest();

                if(result == null)
                {
                    return Ok(new ApiResponseSuccess<IEnumerable<EventRequestResponseDto>>
                    {
                        Message = StringConstant.noEventFound,
                        Data = result
                    });
                }

                return Ok(new ApiResponseSuccess<IEnumerable<EventRequestResponseDto>>
                {
                    Message = StringConstant.eventRequestSuccess,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseError<string>
                {
                    Message = ex.Message
                });
            }
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponseSuccess<EventRequestResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseError<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseError<string>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EventRequestResponseDto>> GetEventRequestById(int id)
        {
            try
            {
                var result = await _eventRequestService.GetEventRequestById(id);

                if (result == null)
                {
                    return NotFound(new ApiResponseError<string>
                    {
                        Message = StringConstant.noRequestFound,
                    });
                }

                return Ok(new ApiResponseSuccess<EventRequestResponseDto>
                {
                    Message = StringConstant.eventRequestSuccess,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseError<string>
                {
                  
                    Message = ex.Message
                });
            }
        }

        [HttpGet("status/{status}")]
        [ProducesResponseType(typeof(ApiResponseSuccess<EventRequestResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseError<string>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<EventRequestResponseDto>>> GetEventRequestByStatus(RequestStatus status)
        {
            try
            {
                var result = await _eventRequestService.GetEventRequestByStatus(status);

                 if(result == null)
                {
                    return Ok(new ApiResponseSuccess<IEnumerable<EventRequestResponseDto>>
                    {
                        Message = StringConstant.noEventFound,
                        Data = result
                    });
                }
                
                return Ok(new ApiResponseSuccess<IEnumerable<EventRequestResponseDto>>
                {
                    Message = StringConstant.eventRequestSuccess,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseError<string>
                {
                    Message = ex.Message
                });
            }
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponseSuccess<EventRequest>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseError<string>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseError<string>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EventRequest>> EditEventRequest(int id, EditEventRequestDto dto)
        {
            try
            {
                var result = await _eventRequestService.EditEventRequest(id, dto);

                 if(result == null)
                {
                    return NotFound(new ApiResponseError<string>
                    {
                        Message = StringConstant.noEventFound,
                    });
                }

                return Ok(new ApiResponseSuccess<EventRequest>
                {
                    Message = StringConstant.eventUpdated,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseError<string>
                {
                    Message = ex.Message
                });
            }
        }

        [HttpPatch("{id:int}/withdraw")]
        [ProducesResponseType(typeof(ApiResponseSuccess<EventRequest>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseError<string>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseError<string>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EventRequest>> WithdrawEventRequest(int id)
        {
            try
            {
                var result = await _eventRequestService.WithdrawlEventRequest(id);

                if(result == null)
                {
                    return NotFound(new ApiResponseError<string>
                    {
                        Message = StringConstant.noRequestFound,
                    });
                }

                return Ok(new ApiResponseSuccess<EventRequest>
                {
                    Message = StringConstant.eventRequestWithdrawl,
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponseError<string>
                {
                    Message = ex.Message
                });
            }
        }
    }
}
