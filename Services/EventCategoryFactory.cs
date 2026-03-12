using SportsManagementApp.Data.Entities;
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

            var genders = gender == GenderType.Both
                ? new[] { GenderType.Male, GenderType.Female }
                : new[] { gender };

            var formats = format == MatchFormat.Both
                ? new[] { MatchFormat.Singles, MatchFormat.Doubles }
                : new[] { format };

            return genders
                .SelectMany(g => formats, (g, f) => Make(g, f, now))
                .ToList();
        }

        private static EventCategory Make(GenderType gender, MatchFormat format, DateTime now) => new()
        {
            Gender    = gender,
            Format    = format,
            Status    = CategoryStatus.Active,
            CreatedAt = now
        };
    }
}