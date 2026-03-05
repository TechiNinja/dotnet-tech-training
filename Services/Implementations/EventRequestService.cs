using SportsManagementApp.Enums;
using SportsManagementApp.Helper;
using SportsManagementApp.Services.Interfaces;
using SportsManagementApp.Repositories.Interfaces;
using AutoMapper;
using SportsManagementApp.Data.DTOs;
using SportsManagementApp.Data.Entities;

namespace SportsManagementApp.Services.EventRequestService.Implementations;

public class EventRequestService : IEventRequestService
{
    private readonly IEventRequestRepository _eventRequestRepository;
    private readonly ISportRepository _sportRepository;
    private readonly IMapper _mapper;

    public EventRequestService(
        IEventRequestRepository eventRequestRepository,
        ISportRepository sportRepository,
        IMapper mapper)
    {
        _eventRequestRepository = eventRequestRepository;
        _sportRepository = sportRepository;
        _mapper = mapper;
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

        if (!await _sportRepository.SportExistsAsync(dto.SportName))
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

        var existingEventRequest = await _eventRequestRepository.ExistsAsync(e => e.SportId == dto.SportId && e.Gender == dto.Gender && e.Format == dto.Format && e.StartDate == dto.StartDate);
        if (existingEventRequest)
        {
            throw new Exception(StringConstant.eventExist);
        }

        var request = _mapper.Map<EventRequest>(dto);
        request.AdminId = adminId;

        await _eventRequestRepository.AddAsync(request);
        await _eventRequestRepository.SaveChangesAsync();

        var createdRequest = await _eventRequestRepository.GetEventRequestById(request.Id);

        return _mapper.Map<EventRequestResponseDto>(createdRequest);
    }

    public async Task<IEnumerable<EventRequestResponseDto>> SearchEventRequests(int? id, RequestStatus? status)
    {
        var eventRequests = await _eventRequestRepository.Search(id, status);
        return _mapper.Map<IEnumerable<EventRequestResponseDto>>(eventRequests);
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

        var request = await _eventRequestRepository.GetEventRequestById(id);

        if (request == null)
        {
            throw new Exception(StringConstant.noRequestFound);
        }

        if (request.Status != RequestStatus.Pending)
        {
            throw new Exception(StringConstant.eventRequestModifyNotAllowed);
        }

        _mapper.Map(dto, request);

        await _eventRequestRepository.UpdateAsync(request);
        await _eventRequestRepository.SaveChangesAsync();

        return _mapper.Map<EventRequestResponseDto>(request);
    }

    public async Task<EventRequestResponseDto> WithdrawEventRequest(int id)
    {
        var request = await _eventRequestRepository.GetEventRequestById(id);

        if (request == null)
        {
            throw new Exception(StringConstant.noRequestFound);
        }

        if (request.Status != RequestStatus.Pending)
        {
            throw new Exception(StringConstant.eventRequestWithdrawlNotAllowed);
        }

        request.Status = RequestStatus.Withdrawn;

        await _eventRequestRepository.UpdateAsync(request);

        await _eventRequestRepository.SaveChangesAsync();

        return _mapper.Map<EventRequestResponseDto>(request);
    }
}