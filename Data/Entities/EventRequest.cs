using SportsManagementApp.Enums;
using System.ComponentModel.DataAnnotations;

namespace SportsManagementApp.Data.Entities
{
    public class EventRequest
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string EventName { get; set; } = string.Empty;
        public int SportId { get; set; }
        public Sport? Sport { get; set; }
        public GenderType Gender { get; set; }
        public MatchFormat Format { get; set; }
        [Required, MaxLength(100)]
        public string RequestedVenue { get; set; } = string.Empty;
        public string LogisticsRequirements { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public RequestStatus Status { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public int AdminId { get; set; }
        public User? Admin { get; set; }
        public int? OperationsReviewerId { get; set; }
        public User? OperationsReviewer { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
