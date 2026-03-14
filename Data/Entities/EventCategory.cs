<<<<<<< HEAD
﻿using SportsManagementApp.Enums;

namespace SportsManagementApp.Data.Entities
{
    public class EventCategory
    {
        public int Id { get; set; }
        public MatchFormat Format { get; set; }
        public GenderType Gender { get; set; }
        public CategoryStatus Status { get; set; }
        public int EventId { get; set; }
        public Event? Event { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<ParticipantRegistration> EventRegistrations { get; set; } = new List<ParticipantRegistration>();
        public ICollection<Team> Teams { get; set; } = new List<Team>();
        public ICollection<Match> Matches { get; set; } = new List<Match>();
    }
}
=======
﻿using SportsManagementApp.Enums;

namespace SportsManagementApp.Data.Entities
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
>>>>>>> 5170b41a9f7db4a9ef1088d78928646be4b14849
