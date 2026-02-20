using Microsoft.EntityFrameworkCore;
using SportsManagementApp.Data;
using SportsManagementApp.Enums;
using SportsManagementApp.Entities;
using SportsManagementApp.DTOs;

namespace SportsManagementApp.Repositories.EventRequestRepository;

public class EventRequestRepository : IEventRequestRepository
{
    private readonly AppDbContext _context;

    public EventRequestRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddEventRequest(EventRequest eventRequest)
    {
        _context.EventRequests.Add(eventRequest);
        await _context.SaveChangesAsync();
    }

   public async Task<List<EventRequestResponseDto>> GetAllEventRequest()
{
    return await _context.EventRequests
        .Select(e => new EventRequestResponseDto
        {
            Id = e.Id,
            EventName = e.EventName,
            SportId = e.SportId,
            SportsName = e.Sport != null ? e.Sport.Name : "",

            Gender = e.Gender,
            Format = e.Format,

            RequestedVenue = e.RequestedVenue,
            LogisticsRequirements = e.LogisticsRequirements,

            StartDate = e.StartDate,
            EndDate = e.EndDate,

            Status = e.Status,
            Remarks = e.Remarks,

            AdminId = e.AdminId,
            OperationsReviewerId = e.OperationsReviewerId,

            CreatedDate = e.CreatedDate,
            UpdatedDate = e.UpdatedDate
        })
        .ToListAsync();
}

public async Task<EventRequestResponseDto?> GetEventRequestById(int id)
{
    return await _context.EventRequests
        .Where(e => e.Id == id)
        .Select(e => new EventRequestResponseDto
        {
            Id = e.Id,
            EventName = e.EventName,
            SportId = e.SportId,
            SportsName = e.Sport != null ? e.Sport.Name : "",

            Gender = e.Gender,
            Format = e.Format,

            RequestedVenue = e.RequestedVenue,
            LogisticsRequirements = e.LogisticsRequirements,

            StartDate = e.StartDate,
            EndDate = e.EndDate,

            Status = e.Status,
            Remarks = e.Remarks,

            AdminId = e.AdminId,
            OperationsReviewerId = e.OperationsReviewerId,

            CreatedDate = e.CreatedDate,
            UpdatedDate = e.UpdatedDate
        })
        .FirstOrDefaultAsync();
}

public async Task<List<EventRequestResponseDto>> GetEventRequestByStatus(RequestStatus status)
{
    return await _context.EventRequests
        .Where(e => e.Status == status)
        .Select(e => new EventRequestResponseDto
        {
            Id = e.Id,
            EventName = e.EventName,
            SportId = e.SportId,
            SportsName = e.Sport != null ? e.Sport.Name : "",

            Gender = e.Gender,
            Format = e.Format,

            RequestedVenue = e.RequestedVenue,
            LogisticsRequirements = e.LogisticsRequirements,

            StartDate = e.StartDate,
            EndDate = e.EndDate,

            Status = e.Status,
            Remarks = e.Remarks,

            AdminId = e.AdminId,
            OperationsReviewerId = e.OperationsReviewerId,

            CreatedDate = e.CreatedDate,
            UpdatedDate = e.UpdatedDate
        })
        .ToListAsync();
}

    public async Task<EventRequest> UpdateEventRequest(EventRequest request)
    {
        _context.EventRequests.Update(request);
        await _context.SaveChangesAsync();
        return request;
    }   

        public async Task<EventRequest?> GetEventRequestEntityById(int id)
        {
        var eventRequest = await _context.EventRequests.FirstOrDefaultAsync(
            x => x.Id == id
        );

        return eventRequest;
    }

    public async Task<List<EventRequest>> GetEventRequestEntityByStatus(RequestStatus status)
    {
        var eventRequest = await _context.EventRequests.Where(
            x => x.Status == status
        ).ToListAsync();

        return eventRequest;
    }

    public async Task<bool> EventRequestExist(CreateEventRequestDto dto)
    {

        var name = dto.EventName.Trim().ToLower();

        return await _context.EventRequests.AnyAsync(e =>
        e.SportId == dto.SportId &&
        e.Gender == dto.Gender &&
        e.Format == dto.Format &&
        e.StartDate == dto.StartDate
    );
    }

         
}