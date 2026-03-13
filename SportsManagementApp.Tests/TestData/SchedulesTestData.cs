using SportsManagementApp.Data.DTOs.Participant;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsManagementApp.Tests.TestData
{
    public static class SchedulesTestData
    {
        public static readonly int ValidUserId = 2;
        public static readonly int NoScheduleUserId = 999;

        public static List<MyScheduleDto> UserSchedulesList() => new()
        {
            new()
            { 
                MatchId = 1,
                MatchDateTime = new DateTime(2026, 3, 12, 10, 0, 0),
                Venue = "Seed Stadium",
                SideA = "Team A",
                SideB = "Team B",
                ScoreA = 2,
                ScoreB = 1,
                EventName = "Badminton Championship"
            },
            new()
            {
                MatchId = 2,
                MatchDateTime = new DateTime(2026, 3, 13, 14, 0, 0),
                Venue = "Main Court",
                SideA = "Team C",
                SideB = "Team D",
                ScoreA = 0,
                ScoreB = 0,
                EventName = "Cricket Finals"
            }
        };

        public static List<MyScheduleDto> EmptySchedulesList() => new();
    }
}
