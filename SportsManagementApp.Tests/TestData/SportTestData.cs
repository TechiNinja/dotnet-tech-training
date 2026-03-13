using SportsManagementApp.Data.DTOs.SportManagement;
using SportsManagementApp.Data.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SportsManagementApp.Tests.TestData
{
    public static class SportTestData
    {
        public static CreateSportDto EmptyNameDto() => new() { Name = "" };
        public static CreateSportDto WhitespaceNameDto() => new() { Name = "   " };
        public static CreateSportDto DuplicateDto() => new() { Name = "Badminton" };
        public static CreateSportDto ValidCreateDto() => new() { Name = "Pool" };

        public static UpdateSportDto EmptyUpdateDto() => new() { Name = "" };
        public static UpdateSportDto NotFoundUpdateDto() => new() { Name = "Cricket" };
        public static UpdateSportDto ConflictingUpdateDto() => new() { Name = "Carrom" };
        public static UpdateSportDto SameNameUpdateDto() => new() { Name = "Badminton" };
        public static UpdateSportDto ValidUpdateDto() => new() { Name = "Tennis" };

        public static Sport ExistingSport() => new() { Id = 1, Name = "Badminton" };
        public static Sport CreatedSport() => new() { Id = 5, Name = "Pool" };

        public static List<SportResponseDto> SportResponseList() => new()
        {
            new() { Id = 1, Name = "Badminton" },
            new() { Id = 2, Name = "Carrom" },
            new() { Id = 3, Name = "Chess" },
            new() { Id = 4, Name = "Foosball" }
        };
    }
}
