using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Repositories.Interfaces
{
    public interface ISportRepository: IGenericRepository<Sport>
    {
        Task<bool> SportExistsAsync(string name);
        Task<List<TResult>> GetSportsAsyncWithFilter<TResult>(
            Expression<Func<Sport, bool>> predicate,
            Expression<Func<Sport, TResult>> projection);
    }
}