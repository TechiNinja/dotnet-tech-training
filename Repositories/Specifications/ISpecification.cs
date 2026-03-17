using System.Linq.Expressions;

namespace SportsManagementApp.Repositories.Specifications
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> ToExpression();
    }
}