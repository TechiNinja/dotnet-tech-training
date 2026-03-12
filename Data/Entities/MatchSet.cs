using SportsManagementApp.Enums;

namespace SportsManagementApp.Data.Entities
{
    public class MatchSet
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public Match? Match { get; set; }
        public int SetNumber { get; set; }
        public int ScoreA { get; set; }
        public int ScoreB { get; set; }
        public SetStatus Status { get; set; } = SetStatus.NotStarted;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
