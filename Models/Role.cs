using SportsManagementApp.Enums;

namespace SportsManagementApp.Models
{
    public class Role
    {
        public int Id { get; set; }
        public RoleType RoleType { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
