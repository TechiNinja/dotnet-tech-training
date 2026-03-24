using System.Collections.Generic;

namespace SportsManagementApp.Data.DTOs.SportManagement
{
    public class SportResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<string> AllowedFormats { get; set; } = new();
    }
}