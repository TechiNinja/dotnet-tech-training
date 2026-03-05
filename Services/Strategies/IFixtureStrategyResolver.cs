using SportsManagementApp.Enums;

namespace SportsManagementApp.Services.Strategies
{
    public interface IFixtureStrategyResolver
    {
        IFixtureStrategy Resolve(TournamentType type);
    }
}
