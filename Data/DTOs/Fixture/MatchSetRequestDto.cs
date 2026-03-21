using System.ComponentModel.DataAnnotations;
using SportsManagementApp.Constants;

namespace SportsManagementApp.DTOs.Fixture
{
    public class MatchSetRequestDto
    {
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = StringConstant.ScoreValidate)]
        public int ScoreA { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = StringConstant.ScoreValidate)]
        public int ScoreB { get; set; }

        public bool IsCompleted { get; set; } = false;
    }
}