using System;
using System.Collections.Generic;
using System.Text;
using SportsManagementApp.Data.DTOs.Participant;

namespace SportsManagementApp.Tests.TestData
{
    public static class EventsTestData
    {
        public static readonly int ValidUserId = 2;
        public static readonly int NoEventsUserId = 999;

        public static List<MyEventsDto> UserEventsList() => new()
        {
            new() { EventId = 8, EventName = "Test Badminton Event", StartDate = new DateOnly(2026, 3, 10), EndDate = new DateOnly(2026, 3, 12) },
            new() { EventId = 9, EventName = "Cricket Tournament", StartDate = new DateOnly(2026, 3, 15), EndDate = new DateOnly(2026, 3, 17) }
        };

        public static List<MyEventsDto> EmptyEventsList() => new();
    }
}
