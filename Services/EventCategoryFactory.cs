using SportsManagementApp.Entities;
using SportsManagementApp.Enums;

namespace SportsManagementApp.Services
{
    public static class EventCategoryFactory
    {
        public static List<EventCategory> Generate(GenderType gender, MatchFormat format)
        {
            var now = DateTime.UtcNow;

            if (gender == GenderType.Mixed)
                return new List<EventCategory> { Make(GenderType.Mixed, MatchFormat.Singles, now) };

            return ExpandGenders(gender)
                .SelectMany(g => ExpandFormats(format), (g, f) => Make(g, f, now))
                .ToList();
        }

        private static IEnumerable<GenderType> ExpandGenders(GenderType gender) => gender switch
        {
            GenderType.Male   => new[] { GenderType.Male },
            GenderType.Female => new[] { GenderType.Female },
            GenderType.Both   => new[] { GenderType.Male, GenderType.Female },
            _                 => Array.Empty<GenderType>()
        };

        private static IEnumerable<MatchFormat> ExpandFormats(MatchFormat format) => format switch
        {
            MatchFormat.Singles => new[] { MatchFormat.Singles },
            MatchFormat.Doubles => new[] { MatchFormat.Doubles },
            MatchFormat.Both    => new[] { MatchFormat.Singles, MatchFormat.Doubles },
            _                   => Array.Empty<MatchFormat>()
        };

        private static EventCategory Make(GenderType gender, MatchFormat format, DateTime now) => new()
        {
            Gender    = gender,
            Format    = format,
            Status    = CategoryStatus.Active,
            CreatedAt = now
        };
    }
}
