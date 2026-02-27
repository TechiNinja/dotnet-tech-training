using System.ComponentModel.DataAnnotations;

namespace SportsManagementApp.Data.DTOs.TeamManagement
{
    public class CreateTeamRequestDto
    {
        [Required]
        public int EventCategoryId { get; set; }
    }
}
