using System.ComponentModel.DataAnnotations;

namespace SportsManagementApp.Data.Entities
{
    public class Team
    {
        public int Id { get; set; }
        [Required, MaxLength(20)]
        public string Name { get; set; } = string.Empty;
        public int EventCategoryId { get; set; }
        public EventCategory? EventCategory { get; set; }
        public ICollection<TeamMember> Members { get; set; } = new List<TeamMember>();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
