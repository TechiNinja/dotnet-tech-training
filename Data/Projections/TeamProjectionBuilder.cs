using SportsManagementApp.Data.DTOs.TeamManagement;
using SportsManagementApp.Data.Entities;
using System.Linq.Expressions;

namespace SportsManagementApp.Data.Projections
{
    public static class TeamProjectionBuilder
    {
        public static Expression<Func<Team, TeamResponseDto>> Build()
        {
            return team => new TeamResponseDto
            {
                Id = team.Id,
                Name = team.Name,
                EventCategoryId = team.EventCategoryId,
                Members = team.Members
                              .Select(member => member.User != null ? member.User.FullName : "N/A")
                              .ToList()
            };
        }
    }
}
