using SportsManagementApp.Enums;
using System.ComponentModel.DataAnnotations;

namespace SportsManagementApp.Data.DTOs;

public class CreateEventRequestDto
{
    [Required, MaxLength(100)]
    public string EventName { get; set; } = null!;
    public int SportId { get; set; }
    public string SportName {get; set;} = string.Empty;

    [Required, MaxLength(100)]
    public string RequestedVenue { get; set; } = null!;

    [MaxLength(500)]
    public string LogisticsRequirements { get; set; } = null!;

    [Required]
    public MatchFormat Format { get; set; }

    [Required]
    public GenderType Gender { get; set; }

    [Required]
    public DateOnly StartDate { get; set; }

    [Required]
    public DateOnly EndDate { get; set; }
}

