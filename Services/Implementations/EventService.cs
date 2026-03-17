using AutoMapper;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Data.Filters;
using SportsManagementApp.DTOs.EventCreation;
using SportsManagementApp.Enums;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;
using SportsManagementApp.Constants;

namespace SportsManagementApp.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepo;
        private readonly IEventRequestRepository _requestRepo;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;

        public EventService(
            IEventRepository eventRepo,
            IEventRequestRepository requestRepo,
            IUserRepository userRepo,
            IMapper mapper)
        {
            _eventRepo   = eventRepo;
            _requestRepo = requestRepo;
            _userRepo    = userRepo;
            _mapper      = mapper;
        }

        public async Task<IEnumerable<EventResponseDto>> GetAllAsync(EventFilterDto filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.Status) &&
                !Enum.TryParse<EventStatus>(filter.Status, true, out _))
                throw new BadRequestException(string.Format(StringConstant.InvalidEventStatus, filter.Status));

            if (filter.EventId.HasValue)
            {
                var single = await _eventRepo.GetByIdWithDetailsAsync(filter.EventId.Value)
                    ?? throw new NotFoundException(string.Format(StringConstant.EventNotFound, filter.EventId));
                return new[] { _mapper.Map<EventResponseDto>(single) };
            }

            return await _eventRepo.GetProjectedListAsync(filter);
        }

        public async Task<EventResponseDto> GetByIdAsync(int eventId)
        {
            var entity = await _eventRepo.GetByIdWithDetailsAsync(eventId)
                ?? throw new NotFoundException(string.Format(StringConstant.EventNotFound, eventId));
            return _mapper.Map<EventResponseDto>(entity);
        }

        public async Task<EventRequestPreFillResponseDto> GetEventRequestForPreFillAsync(int requestId)
        {
            var eventRequest = await _requestRepo.GetEventRequestById(requestId)
                ?? throw new NotFoundException(string.Format(StringConstant.EventRequestNotFound, requestId));

            if (eventRequest.Status != RequestStatus.Approved)
                throw new UnprocessableEntityException(
                    string.Format(StringConstant.EventRequestNotApproved, eventRequest.Status));

            var response = _mapper.Map<EventRequestPreFillResponseDto>(eventRequest);
            response.IsEventAlreadyCreated = await _eventRepo.ExistsByRequestIdAsync(requestId);
            return response;
        }

        public async Task<EventResponseDto> CreateEventFromRequestAsync(CreateEventDto request)
        {
            var eventRequest = await _requestRepo.GetEventRequestById(request.EventRequestId)
                ?? throw new NotFoundException(
                    string.Format(StringConstant.EventRequestNotFound, request.EventRequestId));

            if (request.RegistrationDeadline >= eventRequest.StartDate)
                throw new BadRequestException(StringConstant.RegistrationDeadlineInvalid);

            if (await _eventRepo.ExistsByRequestIdAsync(request.EventRequestId))
                throw new ConflictException(
                    string.Format(StringConstant.EventAlreadyExists, request.EventRequestId));

            var genderTypes  = eventRequest.Gender == GenderType.Both
                ? new[] { GenderType.Male, GenderType.Female }
                : new[] { eventRequest.Gender };

            var matchFormats = eventRequest.Format == MatchFormat.Both
                ? new[] { MatchFormat.Singles, MatchFormat.Doubles }
                : new[] { eventRequest.Format };

            var newEvent = new Event
            {
                EventRequestId       = request.EventRequestId,
                Description          = request.Description,
                MaxParticipantsCount = request.MaxParticipantsCount,
                RegistrationDeadline = request.RegistrationDeadline,
                Name                 = !string.IsNullOrWhiteSpace(request.Name)
                                        ? request.Name
                                        : eventRequest.EventName,
                SportId              = eventRequest.SportId,
                StartDate            = eventRequest.StartDate,
                EndDate              = eventRequest.EndDate,
                EventVenue           = eventRequest.RequestedVenue,
                OrganizerId          = eventRequest.AdminId,
                Status               = EventStatus.Open,
                TournamentType       = TournamentType.Knockout,
                CreatedAt            = DateTime.UtcNow,
                Categories           = genderTypes
                    .SelectMany(genderType => matchFormats.Select(matchFormat => new EventCategory
                    {
                        Gender    = genderType,
                        Format    = matchFormat,
                        Status    = CategoryStatus.Active,
                        CreatedAt = DateTime.UtcNow
                    }))
                    .ToList()
            };

            await _eventRepo.AddAsync(newEvent);

            var created = await _eventRepo.GetByIdWithDetailsAsync(newEvent.Id)
                ?? throw new NotFoundException(string.Format(StringConstant.EventNotFound, newEvent.Id));
            return _mapper.Map<EventResponseDto>(created);
        }

        public async Task<EventResponseDto> PatchEventAsync(int eventId, PatchEventRequestDto request)
        {
            var entity = await _eventRepo.GetByIdWithDetailsAsync(eventId)
                ?? throw new NotFoundException(string.Format(StringConstant.EventNotFound, eventId));

            ValidateEventEditable(entity);

            var action = request.Action.Trim().ToLower();

            if (action == "cancel")
            {
                entity.Status    = EventStatus.Cancelled;
                entity.UpdatedAt = DateTime.UtcNow;
            }
            else if (action == "update")
            {
                _mapper.Map(request, entity);
                entity.UpdatedAt = DateTime.UtcNow;

                if (request.RegistrationDeadline.HasValue)
                {
                    if (request.RegistrationDeadline.Value >= entity.StartDate)
                        throw new BadRequestException(StringConstant.RegistrationDeadlineInvalid);
                    if (entity.Status == EventStatus.Cancelled)
                        entity.Status = EventStatus.Open;
                }
            }
            else
            {
                throw new BadRequestException($"Invalid action '{request.Action}'. Use 'update' or 'cancel'.");
            }

            await _eventRepo.UpdateAsync(entity);

            return _mapper.Map<EventResponseDto>(entity);
        }

        public async Task<EventResponseDto> AssignOrganizerAsync(int eventId, int organizerId)
        {
            var entity = await _eventRepo.GetByIdWithDetailsAsync(eventId)
                ?? throw new NotFoundException(string.Format(StringConstant.EventNotFound, eventId));

            ValidateEventEditable(entity);

            var organizer = await _userRepo.GetUserEntityByIdAsync(organizerId)
                ?? throw new NotFoundException(string.Format(StringConstant.UserNotFound, organizerId));

            if (!organizer.IsActive)
                throw new UnprocessableEntityException(
                    string.Format(StringConstant.UserInactive, organizer.FullName));

            if (organizer.RoleId != StringConstant.OrganizerRoleId)
                throw new UnprocessableEntityException(
                    string.Format(StringConstant.UserNotOrganizer, organizer.FullName));

            entity.OrganizerId = organizer.Id;
            entity.Organizer   = organizer;
            entity.UpdatedAt   = DateTime.UtcNow;

            await _eventRepo.UpdateAsync(entity);

            return _mapper.Map<EventResponseDto>(entity);
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