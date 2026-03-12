using SportsManagementApp.Enums;
namespace SportsManagementApp.Data.DTOs;
public class EventRequestResponseDto
{
    public int Id { get; set; }
    public string EventName { get; set; } = string.Empty;
    public int SportId { get; set; }     
    public string SportsName { get; set;} = string.Empty;  
    public GenderType Gender { get; set; }
    public MatchFormat Format { get; set; }
    public string RequestedVenue { get; set; } = string.Empty;
    public string LogisticsRequirements { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public RequestStatus Status { get; set; }
    public string Remarks { get; set; } = string.Empty;
    public int AdminId { get; set; }      
    public int? OperationsReviewerId { get; set; } 
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}