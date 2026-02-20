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

    public async Task AddEventRequest(EventRequest request)
    {
        _context.EventRequests.Add(request);
        await _context.SaveChangesAsync();
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

    public async Task<IEnumerable<EventRequestResponseDto>> Search(int? id, RequestStatus? status)
    {
        var query = _context.EventRequests.AsNoTracking().AsQueryable();

        if (id.HasValue){
            query = query.Where(r => r.Id == id.Value);
        }

        if (status.HasValue){
            query = query.Where(r => r.Status == status.Value);
        }

        return await query.Select(r => new EventRequestResponseDto
        {
            Id = r.Id,
            EventName = r.EventName,
            SportsName = r.Sport == null ? "" : r.Sport.Name,
            RequestedVenue = r.RequestedVenue,
            LogisticsRequirements = r.LogisticsRequirements,
            Format = r.Format,
            Gender = r.Gender,
            StartDate = r.StartDate,
            EndDate = r.EndDate,
            Status = r.Status,
            SportId = r.SportId
        }).ToListAsync();
    }

    public async Task UpdateEventRequest(EventRequest request)
    {
        _context.EventRequests.Update(request);
        await _context.SaveChangesAsync();

    }

    public async Task<EventRequest?> GetEventRequestEntityById(int id)
    {
        var eventRequest = await _context.EventRequests.FirstOrDefaultAsync(
            x => x.Id == id
        );

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