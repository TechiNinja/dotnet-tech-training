using SportsManagementApp.Enums;

namespace SportsManagementApp.Entities
{
    public class EventCategory
    {
        public int Id { get; set; }
        public MatchFormat Format { get; set; }
        public GenderType Gender { get; set; }
        public CategoryStatus Status { get; set; }
        public TournamentType TournamentType { get; set; } = TournamentType.Knockout;
        public int EventId { get; set; }
        public Event? Event { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<ParticipantRegistration> EventRegistrations { get; set; } = new List<ParticipantRegistration>();
        public ICollection<Team> Teams { get; set; } = new List<Team>();
        public ICollection<Match> Matches { get; set; } = new List<Match>();
    }
}
