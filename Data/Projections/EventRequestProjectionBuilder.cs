using System.Linq.Expressions;
using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Data.Projections
{
    public static class EventRequestProjectionBuilder
    {
        public static Expression<Func<EventRequest, EventRequestResponseDto>> Build()
        {
            return request => new EventRequestResponseDto
            {
                Id = request.Id,
                EventName = request.EventName,
                SportId = request.SportId,
                SportsName = request.Sport != null ? request.Sport.Name : "Unknown",
                Gender = request.Gender,
                Format = request.Format,
                RequestedVenue = request.RequestedVenue,
                LogisticsRequirements = request.LogisticsRequirements,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = request.Status,
                Remarks = request.Remarks,
                AdminId = request.AdminId,
                AdminName = request.Admin != null ? request.Admin.FullName : "Unknown",
                OperationsReviewerId = request.OperationsReviewerId,
                OperationsReviewerName = request.OperationsReviewer != null
                    ? request.OperationsReviewer.FullName
                    : "Not assigned",
                CreatedDate = request.CreatedDate,
                UpdatedDate = request.UpdatedDate
            };
        }
    }
}