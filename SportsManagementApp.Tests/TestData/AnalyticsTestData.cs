using System;
using System.Collections.Generic;
using System.Text;
using SportsManagementApp.Constants;
using SportsManagementApp.Data.DTOs.Analytics;

namespace SportsManagementApp.Tests.TestData
{
    public static class AnalyticsTestData
    {
        public static AdminAnalyticsDto AdminAnalytics() => new()
        {
            TotalEvents = 5,
            ActiveUsers = 21,
            Teams = 4,
            MatchesToday = 2
        };

        public static OrganizerAnalyticsDto OrganizerAnalytics() => new()
        {
            MyEvents = 3,
            TotalRegistrations = 19,
            TeamsRegistered = 4,
            LiveMatches = 1
        };

        public static readonly int ValidUserId = 4;
        public static readonly int AdminUserId = 1;

        public static readonly string AdminRole = RoleConstants.Admin;
        public static readonly string OpsTeamRole = RoleConstants.OpsTeam;
        public static readonly string OrganizerRole = RoleConstants.Organizer;
        public static readonly string ParticipantRole = RoleConstants.Participant;
        public static readonly string InvalidRole = "InvalidRole";
    }
}
