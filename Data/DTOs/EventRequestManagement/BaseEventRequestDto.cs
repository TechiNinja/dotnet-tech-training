using SportsManagementApp.Enums;
using System.ComponentModel.DataAnnotations;

namespace SportsManagementApp.Data.DTOs;

public class BaseEventRequestDto
{
    [Required]
    [MaxLength(100)]
    public string EventName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string RequestedVenue { get; set; } = string.Empty;

    [MaxLength(500)]
    public string LogisticsRequirements { get; set; } = string.Empty;

    [Required]
    public MatchFormat Format { get; set; }

    [Required]
    public GenderType Gender { get; set; }

    [Required]
    public DateOnly StartDate { get; set; }

    [Required]
    public DateOnly EndDate { get; set; }
}