using System.ComponentModel.DataAnnotations;

namespace SportsManagementApp.Data.DTOs;

public class CreateEventRequestDto : BaseEventRequestDto
{
    [Required]
    public int SportId { get; set; }
}