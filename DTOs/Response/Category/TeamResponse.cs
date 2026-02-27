namespace SportsManagementApp.DTOs.Response
{
    public class TeamResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<TeamMemberResponse> Members { get; set; } = new();
        public DateTime CreatedAt { get; set; }
    }

    public class TeamMemberResponse
    {
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}