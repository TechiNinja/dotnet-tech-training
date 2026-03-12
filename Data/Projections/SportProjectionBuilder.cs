using SportsManagementApp.Data.DTOs.SportManagement;
using SportsManagementApp.Data.Entities;
using System.Linq.Expressions;

namespace SportsManagementApp.Data.Projections
{
    public static class SportProjectionBuilder
    {
        public static Expression<Func<Sport, SportResponseDto>> Build()
        {
            return sport => new SportResponseDto
            {
                Id = sport.Id,
                Name = sport.Name
            };
        }
    }
}
