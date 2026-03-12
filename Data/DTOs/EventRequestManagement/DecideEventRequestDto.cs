using System.ComponentModel.DataAnnotations;

namespace SportsManagementApp.Data.DTOs
{
    public class DecideEventRequestDto
    {
        [Required]
        [MaxLength(500)]
        public string Remarks { get; set; } = string.Empty;
    }
}