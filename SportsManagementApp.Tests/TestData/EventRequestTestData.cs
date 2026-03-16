using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Enums;

namespace SportsManagementApp.Tests.TestData;

public static class EventRequestTestData
{
    public static CreateEventRequestDto ValidCreateDto => new()
    {
        EventName = "Football Tournament",
        SportId = 1,
        RequestedVenue = "Ground B",
        LogisticsRequirements = "Lights",
        Format = MatchFormat.Singles,
        Gender = GenderType.Female,
        StartDate = new DateOnly(2026, 4, 1),
        EndDate = new DateOnly(2026, 4, 3)
    };

   public static CreateEventRequestDto InvalidDateDto => new()
{
    EventName = "Cricket",
    SportId = 1,
    RequestedVenue = "Ground A",
    LogisticsRequirements = "Water",
    Format = MatchFormat.Singles,
    Gender = GenderType.Male,
    StartDate = new DateOnly(2026, 3, 20),
    EndDate = new DateOnly(2026, 3, 18)
};

    public static CreateEventRequestDto DuplicateEventDto => new()
    {
        EventName = "Cricket",
        SportId = 1,
        RequestedVenue = "Ground A",
        LogisticsRequirements = "Water",
        Format = MatchFormat.Both,
        Gender = GenderType.Male,
        StartDate = new DateOnly(2026, 3, 18),
        EndDate = new DateOnly(2026, 3, 20)
    };

    public static EditEventRequestDto EditDto => new()
    {
        EventName = "Updated Event",
        RequestedVenue = "Ground C",
        LogisticsRequirements = "Mic",
        Format = MatchFormat.Singles,
        Gender = GenderType.Male,
        StartDate = new DateOnly(2026, 5, 1),
        EndDate = new DateOnly(2026, 5, 2)
    };

    public static EventRequest SampleEventRequest => new()
    {
        Id = 101,
        SportId = 2,
        Format = MatchFormat.Singles,
        Gender = GenderType.Female,
        EventName = "Football Tournament",
        RequestedVenue = "Ground B",
        Status = RequestStatus.Pending,
        AdminId = 10
    };

    public static EventRequestResponseDto SampleResponseDto => new()
    {
        AdminId = 10,
        EventName = "Football Tournament",
        RequestedVenue = "Ground B",
        Format = MatchFormat.Singles,
        Gender = GenderType.Female,
        StartDate = new DateOnly(2026, 4, 1),
        EndDate = new DateOnly(2026, 4, 3),
        Status = RequestStatus.Pending
    };
}