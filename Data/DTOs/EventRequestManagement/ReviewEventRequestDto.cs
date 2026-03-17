using System.ComponentModel.DataAnnotations;
using SportsManagementApp.Enums;

namespace SportsManagementApp.Data.DTOs
{
    public class ReviewEventRequestDto
    {
        [Required]
        public RequestStatus Status {get;set;}

        [MaxLength(500)]
        public string? Remarks { get; set; } = string.Empty;
    }
}