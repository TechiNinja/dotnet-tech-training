namespace SportsManagementApp.Data.DTOs.TeamManagement
{
    public class TeamResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<string> Members { get; set; } = [];
        public int EventCategoryId { get; set; }
    }
}
