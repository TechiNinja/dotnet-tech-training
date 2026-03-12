using System.ComponentModel.DataAnnotations;
using SportsManagementApp.StringConstants;

namespace SportsManagementApp.DTOs.Request
{
    public class MatchSetRequest
    {
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = AppConstants.ScoreValidate)]
        public int ScoreA { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = AppConstants.ScoreValidate)]
        public int ScoreB { get; set; }

        public bool IsCompleted { get; set; } = false;
    }
}
