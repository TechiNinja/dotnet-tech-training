using SportsManagementApp.Data.DTOs.Participant;
using SportsManagementApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsManagementApp.Tests.TestData
{
    public static class ParticipantRegistrationTestData
    {
        public static ParticipantRegistrationRequestDto ValidRegistrationRequest() => new()
        { UserId = 2, EventCategoryId = 8 };

        public static ParticipantRegistrationRequestDto DuplicateRegistrationRequest() => new()
        { UserId = 6, EventCategoryId = 8 };

        public static ParticipantRegistration MappedRegistration() => new()
        { UserId = 2, EventCategoryId = 8 };

        public static ParticipantRegistration SavedRegistrationWithUser() => new()
        { Id = 20, UserId = 2, EventCategoryId = 8, User = new User { FullName = "Vishal Kumar" } };

        public static List<ParticipantRegistration> CategoryRegistrationsList() => new()
        {
            new() { Id = 13, UserId = 6, EventCategoryId = 8, User = new User { FullName = "Vaishak" } },
            new() { Id = 15, UserId = 2, EventCategoryId = 8, User = new User { FullName = "Vishal Kumar" } }
        };

        public static ParticipantRegistrationResponseDto SuccessfulRegistrationResponse() => new()
        { Id = 20, UserId = 2, Name = "Vishal Kumar", EventCategoryId = 8, RegisteredAt = DateTime.UtcNow };

        public static List<ParticipantRegistrationResponseDto> CategoryRegistrationsResponseList() => new()
        {
            new() { Id = 13, UserId = 6, Name = "Vaishak", EventCategoryId = 8 },
            new() { Id = 15, UserId = 2, Name = "Vishal Kumar", EventCategoryId = 8 }
        };

        public static List<ParticipantRegistrationResponseDto> EmptyRegistrationsResponseList() => new();

        public static readonly int ValidCategoryId = 8;
        public static readonly int EmptyCategoryId = 999;
    }
}
