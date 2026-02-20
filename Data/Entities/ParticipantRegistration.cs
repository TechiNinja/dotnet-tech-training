using SportsManagementApp.Enums;
using System.ComponentModel.DataAnnotations;

namespace SportsManagementApp.Data.Entities
{
    public class ParticipantRegistration
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public int EventCategoryId { get; set; }
        public EventCategory? EventCategory { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
