using System.ComponentModel.DataAnnotations;
using SportsManagementApp.StringConstants;

namespace SportsManagementApp.DTOs.EventCreation
{
    public class PatchEventRequestDto
    {
        [Required]
        public string Action { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = StringConstant.EventNameTooLong)]
        public string? Name { get; set; }

        [MaxLength(1000, ErrorMessage = StringConstant.DescriptionTooLong)]
        public string? Description { get; set; }

        [Range(2, 10000, ErrorMessage = StringConstant.MaxParticipantsRange)]
        public int? MaxParticipantsCount { get; set; }

        public DateOnly? RegistrationDeadline { get; set; }
    }
}