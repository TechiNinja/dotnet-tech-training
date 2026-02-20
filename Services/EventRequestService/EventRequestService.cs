using SportsManagementApp.DTOs;
using SportsManagementApp.Enums;
using SportsManagementApp.Entities;
using SportsManagementApp.Repositories;
using SportsManagementApp.Repositories.SportsRepository;
using SportsManagementApp.Helper;

namespace SportsManagementApp.Services.EventRequestService;

public class EventRequestService : IEventRequestService
{
    private readonly IEventRequestRepository _eventRequestRepository;
    private readonly ISportRepository _sportRepository;

    public EventRequestService(IEventRequestRepository eventRequestRepository, ISportRepository sportRepository)
    {
        _eventRequestRepository = eventRequestRepository;
        _sportRepository = sportRepository;
    }

    public async Task<EventRequestResponseDto> RaiseEventRequest(CreateEventRequestDto dto, int adminId)
    {

        if (dto.StartDate == default || dto.EndDate == default)
        {
            throw new Exception(StringConstant.DateRequired);
        }

        if (dto.StartDate > dto.EndDate)
        {
            throw new Exception(StringConstant.DateCompare);
        }

        if (!await _sportRepository.Exists(dto.SportId))
        {
            throw new Exception(StringConstant.invalidSportsId);
        }

        if (dto.Format == MatchFormat.Unknown)
        {
            throw new Exception(StringConstant.MatchFormatRequired);
        }

        if (dto.Gender == GenderType.Unknown)
        {
            throw new Exception(StringConstant.GenderTypeRequired);
        }

        var existingEventRequest = await _eventRequestRepository.EventRequestExist(dto);
        if (existingEventRequest)
        {
            throw new Exception(StringConstant.eventExist);
        }


        var request = new EventRequest
        {
            EventName = dto.EventName.Trim(),
            SportId = dto.SportId,
            RequestedVenue = dto.RequestedVenue.Trim(),
            LogisticsRequirements = dto.LogisticsRequirements,

            Format = dto.Format,
            Gender = dto.Gender,

            StartDate = dto.StartDate,
            EndDate = dto.EndDate,

            Status = RequestStatus.Pending,
            AdminId = adminId,
            CreatedDate = DateTime.UtcNow

        };

        await _eventRequestRepository.AddEventRequest(request);

        var eventRequest = await _eventRequestRepository.GetEventRequestById(request.Id);

        if (eventRequest == null)
        {
            throw new Exception(StringConstant.noEventFound);
        }
        return eventRequest;
    }


    public async Task<IEnumerable<EventRequestResponseDto>> SearchEventRequests(int? id, RequestStatus? status)
    {
        return await _eventRequestRepository.Search(id, status);
    }

    public async Task<EventRequestResponseDto> EditEventRequest(int id, EditEventRequestDto dto)
    {
        if (dto.StartDate == default || dto.EndDate == default)
        {
            throw new Exception(StringConstant.DateRequired);
        }

        if (dto.StartDate > dto.EndDate)
        {
            throw new Exception(StringConstant.DateCompare);
        }

        if (dto.Format == MatchFormat.Unknown)
        {
            throw new Exception(StringConstant.MatchFormatRequired);
        }

        if (dto.Gender == GenderType.Unknown)
        {
            throw new Exception(StringConstant.GenderTypeRequired);
        }

        var request = await _eventRequestRepository.GetEventRequestEntityById(id);

        if (request == null)
        {
            throw new Exception(StringConstant.noRequestFound);
        }

        if (request.Status != RequestStatus.Pending)
        {
            throw new Exception(StringConstant.eventRequestModifyNotAllowed);
        }

        request.EventName = dto.EventName.Trim();
        request.RequestedVenue = dto.RequestedVenue.Trim();
        request.LogisticsRequirements = dto.LogisticsRequirements;

        request.Format = dto.Format;
        request.Gender = dto.Gender;

        request.StartDate = dto.StartDate;
        request.EndDate = dto.EndDate;

        await _eventRequestRepository.UpdateEventRequest(request);

        var eventRequest = await _eventRequestRepository.GetEventRequestById(request.Id);

        if (eventRequest == null)
        {
            throw new Exception(StringConstant.noEventFound);
        }
        return eventRequest;
    }

    public async Task<EventRequestResponseDto> WithdrawlEventRequest(int id)
    {
        var request = await _eventRequestRepository.GetEventRequestEntityById(id);

        if (request == null)
        {
            throw new Exception(StringConstant.noRequestFound);
        }

        if (request.Status != RequestStatus.Pending)
        {
            throw new Exception(StringConstant.eventRequestWithdrawlNotAllowed);
        }

        request.Status = RequestStatus.Withdrawn;

        await _eventRequestRepository.UpdateEventRequest(request);

        var eventRequest = await _eventRequestRepository.GetEventRequestById(request.Id);

        if (eventRequest == null)
        {
            throw new Exception(StringConstant.noEventFound);
        }
        return eventRequest;


    }
}
