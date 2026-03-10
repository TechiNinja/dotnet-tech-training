using SportsManagementApp.Data.Entities;
using SportsManagementApp.Data.Filters;
using System.Linq.Expressions;

namespace SportsManagementApp.Data.Predicates
{
    public static class UserPredicateBuilder
    {
        public static Expression<Func<User, bool>> Build(UserFilterDto filter)
        {
            return user =>
                (!filter.IsActive.HasValue || user.IsActive == filter.IsActive.Value) &&
                (!filter.RoleId.HasValue || user.RoleId == filter.RoleId.Value) &&
                (string.IsNullOrEmpty(filter.SearchTerm) || user.FullName == filter.SearchTerm || user.Email == filter.SearchTerm);
        }
    }
}
