using System.ComponentModel.DataAnnotations;
using SportsManagementApp.Constants;

namespace SportsManagementApp.DTOs.Fixture
{
    public class RescheduleRequestDto
    {
        [Required(ErrorMessage = StringConstant.MatchDateTimeRequired)]
        public DateTime NewStartDateTime { get; set; }
    }
}