using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SportsManagementApp.Data.DTOs.Participant;
using SportsManagementApp.Services.Interfaces;

namespace SportsManagementApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ParticipantRegistrationsController : ControllerBase
    {
        private readonly IParticipantRegistrationService _service;

        public ParticipantRegistrationsController(IParticipantRegistrationService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<ParticipantRegistrationResponseDto>> Register([FromBody] ParticipantRegistrationRequestDto request)
        {
            var result = await _service.RegisterParticipantAsync(request);
            return Ok(result);
        }

        [HttpGet("{categoryId}")]
        public async Task<ActionResult<List<ParticipantRegistrationResponseDto>>> GetByCategory(int categoryId)
        {
            var result = await _service.GetRegistrationsByCategoryAsync(categoryId);
            return Ok(result);
        }
    }
}