using SportsManagementApp.Enums;

namespace SportsManagementApp.Services.Strategies
{
    public sealed class FixtureStrategyResolver : IFixtureStrategyResolver
    {
        private readonly IReadOnlyDictionary<TournamentType, IFixtureStrategy> _strategies;

        public FixtureStrategyResolver(IEnumerable<IFixtureStrategy> strategies)
            => _strategies = strategies.ToDictionary(s => s.TournamentType);

        public IFixtureStrategy Resolve(TournamentType type)
            => _strategies.TryGetValue(type, out var strategy)
                ? strategy
                : throw new InvalidOperationException(
                    $"No fixture strategy registered for tournament type '{type}'.");
    }
}
