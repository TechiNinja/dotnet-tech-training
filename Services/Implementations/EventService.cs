using AutoMapper;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Data.Filters;
using SportsManagementApp.Data.Predicates;
using SportsManagementApp.DTOs.EventCreation;
using SportsManagementApp.Enums;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;
using SportsManagementApp.StringConstants;

namespace SportsManagementApp.Services
{
    public class EventService : GenericService<Event>, IEventService
    {
        private readonly IEventRequestRepository _requestRepo;
        private readonly IUserRepository _userRepo;

        public EventService(
            IEventRepository eventRepo,
            IEventRequestRepository requestRepo,
            IUserRepository userRepo,
            IMapper mapper)
            : base(eventRepo, mapper)
        {
            _requestRepo = requestRepo;
            _userRepo = userRepo;
        }

        public async Task<IEnumerable<EventResponseDto>> GetAllAsync(EventFilterDto filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.Status) &&
                !Enum.TryParse<EventStatus>(filter.Status, true, out _))
                throw new BadRequestException(string.Format(StringConstant.InvalidEventStatus, filter.Status));

            var predicate = EventPredicateBuilder.Build(filter);
            var events = await _repository.GetAllAsync(predicate);
            return _mapper.Map<IEnumerable<EventResponseDto>>(events);
        }

        public async Task<EventRequestPreFillResponseDto> GetEventRequestForPreFillAsync(int requestId)
        {
            var eventRequest = await _requestRepo.GetByIdAsync(requestId)
                ?? throw new NotFoundException(string.Format(StringConstant.EventRequestNotFound, requestId));

            if (eventRequest.Status != RequestStatus.Approved)
                throw new UnprocessableEntityException(
                    string.Format(StringConstant.EventRequestNotApproved, eventRequest.Status));

            var response = _mapper.Map<EventRequestPreFillResponseDto>(eventRequest);
            response.IsEventAlreadyCreated = await _repository.ExistsAsync(e => e.EventRequestId == requestId);
            return response;
        }

        public async Task<EventResponseDto> CreateEventFromRequestAsync(CreateEventDto request)
        {
            var eventRequest = await _requestRepo.GetByIdAsync(request.EventRequestId)
                ?? throw new NotFoundException(
                    string.Format(StringConstant.EventRequestNotFound, request.EventRequestId));

            if (eventRequest.Status != RequestStatus.Approved)
                throw new UnprocessableEntityException(
                    string.Format(StringConstant.EventRequestNotApproved, eventRequest.Status));

            if (request.RegistrationDeadline >= eventRequest.StartDate)
                throw new BadRequestException(StringConstant.RegistrationDeadlineInvalid);

            if (await _repository.ExistsAsync(e => e.EventRequestId == request.EventRequestId))
                throw new ConflictException(
                    string.Format(StringConstant.EventAlreadyExists, request.EventRequestId));

            var newEvent = BuildEventFromRequest(request, eventRequest);

            await _repository.AddAsync(newEvent);
            await _repository.SaveChangesAsync();

            return await GetByIdAsync<EventResponseDto>(newEvent.Id);
        }

        public async Task<EventResponseDto> PatchEventAsync(int eventId, PatchEventRequestDto request)
        {
            var entity = await _repository.GetByIdAsync(eventId)
                ?? throw new NotFoundException(string.Format(StringConstant.EventNotFound, eventId));

            ValidateEventEditable(entity);

            var action = request.Action.Trim().ToLower();

            if (action == StringConstant.ActionCancel)
            {
                entity.Status = EventStatus.Cancelled;
                entity.UpdatedAt = DateTime.UtcNow;
            }
            else if (action == StringConstant.ActionUpdate)
            {
                if (request.RegistrationDeadline.HasValue &&
                    request.RegistrationDeadline.Value >= entity.StartDate)
                    throw new BadRequestException(StringConstant.RegistrationDeadlineInvalid);

                _mapper.Map(request, entity);
                entity.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                throw new BadRequestException(
                    $"Invalid action '{request.Action}'. Use '{StringConstant.ActionUpdate}' or '{StringConstant.ActionCancel}'.");
            }

            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();

            return _mapper.Map<EventResponseDto>(entity);
        }

        public async Task<EventResponseDto> AssignOrganizerAsync(int eventId, int organizerId)
        {
            var entity = await _repository.GetByIdAsync(eventId)
                ?? throw new NotFoundException(string.Format(StringConstant.EventNotFound, eventId));

            ValidateEventEditable(entity);

            var organizer = await _userRepo.GetUserEntityByIdAsync(organizerId)
                ?? throw new NotFoundException(string.Format(StringConstant.UserNotFound, organizerId));

            if (!organizer.IsActive)
                throw new UnprocessableEntityException(
                    string.Format(StringConstant.UserInactive, organizer.FullName));

            if (organizer.RoleId != (int)RoleType.Organizer)
                throw new UnprocessableEntityException(
                    string.Format(StringConstant.UserNotOrganizer, organizer.FullName));

            entity.OrganizerId = organizer.Id;
            entity.Organizer = organizer;
            entity.UpdatedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(entity);
            await _repository.SaveChangesAsync();

            return _mapper.Map<EventResponseDto>(entity);
        }

        private static Event BuildEventFromRequest(CreateEventDto request, EventRequest eventRequest)
        {
            var genderTypes = eventRequest.Gender == GenderType.Both
                ? new[] { GenderType.Male, GenderType.Female }
                : new[] { eventRequest.Gender };

            var matchFormats = eventRequest.Format == MatchFormat.Both
                ? new[] { MatchFormat.Singles, MatchFormat.Doubles }
                : new[] { eventRequest.Format };

            return new Event
            {
                EventRequestId = request.EventRequestId,
                Name = !string.IsNullOrWhiteSpace(request.Name) ? request.Name : eventRequest.EventName,
                Description = request.Description,
                MaxParticipantsCount = request.MaxParticipantsCount,
                RegistrationDeadline = request.RegistrationDeadline,
                SportId = eventRequest.SportId,
                StartDate = eventRequest.StartDate,
                EndDate = eventRequest.EndDate,
                EventVenue = eventRequest.RequestedVenue,
                OrganizerId = eventRequest.AdminId,
                Status = EventStatus.Upcoming,
                TournamentType = TournamentType.Knockout,
                CreatedAt = DateTime.UtcNow,
                Categories = genderTypes
                    .SelectMany(gender => matchFormats.Select(format => new EventCategory
                    {
                        Gender = gender,
                        Format = format,
                        Status = CategoryStatus.Active,
                        CreatedAt = DateTime.UtcNow
                    }))
                    .ToList()
            };
        }

        private static void ValidateEventEditable(Event entity)
        {
            if (entity.Status == EventStatus.Completed)
                throw new UnprocessableEntityException(StringConstant.EventCompleted);
            if (entity.Status == EventStatus.Cancelled)
                throw new UnprocessableEntityException(StringConstant.EventCancelled);
        }
    }
}