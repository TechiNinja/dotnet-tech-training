using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Enums;

namespace SportsManagementApp.Tests.TestData;

public static class OperationsServiceTestData
{
    public static DecideEventRequestDto ApprovalDecisionDto => new()
    {
        Remarks = "approved"
    };

    public static DecideEventRequestDto TrimmedApprovalDecisionDto => new()
    {
        Remarks = "  Looks good  "
    };

    public static DecideEventRequestDto TrimmedRejectionDecisionDto => new()
    {
        Remarks = "  Slot not available  "
    };

    public static DecideEventRequestDto TrimmedOpsApprovalDto => new()
    {
        Remarks = "  Approved by ops  "
    };

    public static DecideEventRequestDto SimpleDecisionDto => new()
    {
        Remarks = "ok"
    };

    public static EventRequest ApprovedRequest => new()
    {
        Id = 1,
        AdminId = 5,
        Status = RequestStatus.Approved
    };

    public static EventRequest PendingRequestForApproval => new()
    {
        Id = 1,
        AdminId = 5,
        Status = RequestStatus.Pending
    };

    public static EventRequest PendingRequestForRejection => new()
    {
        Id = 2,
        AdminId = 7,
        Status = RequestStatus.Pending
    };

    public static EventRequest PendingRequestForTrimCheck => new()
    {
        Id = 3,
        AdminId = 11,
        Status = RequestStatus.Pending
    };

    public static EventRequest PendingRequestForReviewerCheck => new()
    {
        Id = 4,
        AdminId = 14,
        Status = RequestStatus.Pending
    };

    public static EventRequestResponseDto SampleResponseDto => new()
    {
        AdminId = 5,
        EventName = "Football Tournament",
        RequestedVenue = "Ground B",
        Format = MatchFormat.Singles,
        Gender = GenderType.Female,
        StartDate = new DateOnly(2026, 4, 1),
        EndDate = new DateOnly(2026, 4, 3),
        Status = RequestStatus.Pending
    };
}