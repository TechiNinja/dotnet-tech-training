namespace SportsManagementApp.Data.DTOs.UserManagement
{
    public class UpdateUserDto
    {
        public string? FullName { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
        public int? RoleId { get; set; }
        public bool? IsActive { get; set; }
    }
}
