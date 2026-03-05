using System.ComponentModel.DataAnnotations;

namespace SportsManagementApp.Data.DTOs.Participant
{
    public class ParticipantRegistrationRequestDto
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int EventCategoryId { get; set; }
    }
}
