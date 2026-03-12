namespace SportsManagementApp.DTOs.Response
{
    public class EventCategoryResponse
    {
        public int Id { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
