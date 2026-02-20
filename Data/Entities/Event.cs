using SportsManagementApp.Enums;
using System.ComponentModel.DataAnnotations;

namespace SportsManagementApp.Data.Entities
{
    public class Event
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        public int SportId { get; set; }
        public Sport? Sport { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        [Required, MaxLength(200)]
        public string EventVenue { get; set; } = string.Empty;
        public DateOnly RegistrationDeadline { get; set; }
        public int OrganizerId { get; set; }
        public User? Organizer { get; set; }
        public EventStatus Status { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public ICollection<EventCategory> Categories { get; set; } = new List<EventCategory>();
    }
}
