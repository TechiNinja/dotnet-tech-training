using SportsManagementApp.Enums;
using System.ComponentModel.DataAnnotations;

namespace SportsManagementApp.Entities
{
    public class Match
    {
        public int Id { get; set; }
        public int EventCategoryId { get; set; }
        public EventCategory? EventCategory { get; set; }
        public DateTime MatchDateTime { get; set; }
        [Required, MaxLength(200)]
        public string MatchVenue { get; set; } = string.Empty;
        public int? SideAId { get; set; }
        public int? SideBId { get; set; }
        public MatchStatus Status { get; set; } = MatchStatus.Upcoming;
        public int RoundNumber { get; set; }
        public int MatchNumber { get; set; }
        public int BracketPosition { get; set; }
        public ICollection<MatchSet> MatchSets { get; set; } = new List<MatchSet>();
        public Result? Result { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
