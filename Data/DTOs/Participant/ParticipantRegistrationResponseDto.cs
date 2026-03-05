namespace SportsManagementApp.Data.DTOs.Participant
{
    public class ParticipantRegistrationResponseDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int EventCategoryId { get; set; }
        public DateTime RegisteredAt { get; set; }
    }
}
