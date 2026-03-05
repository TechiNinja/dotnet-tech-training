using System.ComponentModel.DataAnnotations;
using SportsManagementApp.StringConstants;

namespace SportsManagementApp.DTOs.Request
{
    public class AssignOrganizerRequest
    {
        [Required(ErrorMessage = AppConstants.RequiredValueMissing)]
        public int OrganizerId { get; set; }
    }
}
