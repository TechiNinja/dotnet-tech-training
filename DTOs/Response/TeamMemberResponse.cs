namespace SportsManagementApp.DTOs.Response
{
    public class TeamMemberResponse
    {
        public int    UserId   { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email    { get; set; } = string.Empty;
    }
}
