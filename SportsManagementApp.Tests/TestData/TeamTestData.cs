using SportsManagementApp.Data.DTOs.TeamManagement;
using SportsManagementApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsManagementApp.Tests.TestData
{
    public static class TeamTestData
    {
        public static CreateTeamRequestDto ValidCreateTeamRequest() => new() { EventCategoryId = 7 };

        public static List<ParticipantRegistration> NoParticipants() => new();

        public static List<ParticipantRegistration> OneParticipant() => new()
        {
            new() { Id = 13, UserId = 6, EventCategoryId = 8 }
        };

        public static List<ParticipantRegistration> TwoParticipants() => new()
        {
            new() { Id = 18, UserId = 10, EventCategoryId = 7 },
            new() { Id = 19, UserId = 11, EventCategoryId = 7 }
        };

        public static List<ParticipantRegistration> ThreeParticipants() => new()
        {
            new() { Id = 13, UserId = 6, EventCategoryId = 8 },
            new() { Id = 14, UserId = 7, EventCategoryId = 8 },
            new() { Id = 15, UserId = 2, EventCategoryId = 8 }
        };

        public static List<ParticipantRegistration> FourParticipants() => new()
        {
            new() { Id = 13, UserId = 6, EventCategoryId = 8 },
            new() { Id = 14, UserId = 7, EventCategoryId = 8 },
            new() { Id = 15, UserId = 2, EventCategoryId = 8 },
            new() { Id = 16, UserId = 8, EventCategoryId = 8 }
        };

        public static List<TeamResponseDto> TeamResponseList() => new()
        {
            new() { Id = 13, Name = "Team 1", EventCategoryId = 8 },
            new() { Id = 14, Name = "Team 2", EventCategoryId = 8 }
        };
    }
}
