using System.ComponentModel.DataAnnotations;

namespace SportsManagementApp.Entities
{
    public class User
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string FullName { get; set; } = string.Empty;
        [Required, MaxLength(100)]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public Role? Role { get; set; }
        public bool IsActive { get; set; } = true;
        public ICollection<ParticipantRegistration> Registrations { get; set; } = new List<ParticipantRegistration>();
        public ICollection<TeamMember> TeamMembers { get; set; } = new List<TeamMember>();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
