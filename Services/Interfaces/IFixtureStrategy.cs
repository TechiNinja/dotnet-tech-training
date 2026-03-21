using SportsManagementApp.Data.Entities;
using SportsManagementApp.Enums;

namespace SportsManagementApp.Services.Strategies
{
    public interface IFixtureStrategy
    {
        TournamentType TournamentType { get; }
        List<Match> Generate(List<int?> sideIds, int categoryId);
    }
}