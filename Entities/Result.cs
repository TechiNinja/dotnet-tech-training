using SportsManagementApp.Enums;

namespace SportsManagementApp.Entities
{
    public class Result
    {
        public int Id { get; set; }
        public int MatchId { get; set; }
        public Match? Match { get; set; }
        public int? WinnerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
