using SportsManagementApp.Enums;

namespace SportsManagementApp.Data.DTOs;

public class EventRequestResponseDto : BaseEventRequestDto
{
    public int Id { get; set; }

    public int SportId { get; set; }
    public string SportsName { get; set; } = string.Empty;

    public RequestStatus Status { get; set; }
    public string Remarks { get; set; } = string.Empty;

    public int AdminId { get; set; }
    public string AdminName { get; set; } = string.Empty;

    public int? OperationsReviewerId { get; set; }
    public string? OperationsReviewerName { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}