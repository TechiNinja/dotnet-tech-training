using SportsManagementApp.Data.Entities;
using SportsManagementApp.Data.Filters;
using System.Linq.Expressions;

namespace SportsManagementApp.Data.Predicates
{
    public static class TeamPredicateBuilder
    {
        public static Expression<Func<Team, bool>> Build(TeamFilterDto filter)
        {
            return team =>
                (!filter.UserId.HasValue || team.Members.Any(member => member.UserId == filter.UserId.Value)) &&
                (!filter.CategoryId.HasValue || team.EventCategoryId == filter.CategoryId.Value);
        }
    }
}
