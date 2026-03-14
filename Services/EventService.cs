using AutoMapper;
using SportsManagementApp.DTOs.Request;
using SportsManagementApp.DTOs.Response;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Enums;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;
using SportsManagementApp.StringConstants;

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

        public async Task<IEnumerable<EventResponse>> GetAllAsync(EventFilterRequest filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.Status) &&
                !Enum.TryParse<EventStatus>(filter.Status, true, out _))
                throw new BadRequestException(string.Format(AppConstants.InvalidEventStatus, filter.Status));

            if (filter.Id.HasValue)
            {
                var single = await _eventRepo.GetByIdWithDetailsAsync(filter.Id.Value)
                    ?? throw new NotFoundException(string.Format(AppConstants.EventNotFound, filter.Id));
                return new[] { _mapper.Map<EventResponse>(single) };
            }

            return await _eventRepo.GetProjectedListAsync(filter.Status, filter.Name, filter.SportId);
        }

        public async Task<EventResponse> GetByIdAsync(int eventId)
        {
            var entity = await _eventRepo.GetByIdWithDetailsAsync(eventId)
                ?? throw new NotFoundException(string.Format(AppConstants.EventNotFound, eventId));
            return _mapper.Map<EventResponse>(entity);
        }

        public async Task<EventRequestPreFillResponse> GetEventRequestForPreFillAsync(int requestId)
        {
            var eventRequest = await _requestRepo.GetByIdWithDetailsAsync(requestId)
                ?? throw new NotFoundException(string.Format(AppConstants.EventRequestNotFound, requestId));

            if (eventRequest.Status != RequestStatus.Approved)
                throw new UnprocessableEntityException(
                    string.Format(AppConstants.EventRequestNotApproved, eventRequest.Status));

            bool alreadyCreated = await _eventRepo.ExistsByRequestIdAsync(requestId);

            return new EventRequestPreFillResponse
            {
                Id                    = eventRequest.Id,
                SportName             = eventRequest.Sport?.Name ?? string.Empty,
                Gender                = eventRequest.Gender.ToString(),
                Format                = eventRequest.Format.ToString(),
                RequestedVenue        = eventRequest.RequestedVenue,
                StartDate             = eventRequest.StartDate,
                EndDate               = eventRequest.EndDate,
                Status                = eventRequest.Status.ToString(),
                IsEventAlreadyCreated = alreadyCreated,
                Name                  = eventRequest.EventName,
                Description           = null,
                RegistrationDeadline  = null
            };
        }

        public async Task<EventResponse> CreateEventFromRequestAsync(CreateEventRequest request)
        {
            var eventRequest = await _requestRepo.GetByIdWithDetailsAsync(request.EventRequestId)
                ?? throw new NotFoundException(
                    string.Format(AppConstants.EventRequestNotFound, request.EventRequestId));

            ValidateEventCreation(eventRequest, request);

            if (await _eventRepo.ExistsByRequestIdAsync(request.EventRequestId))
                throw new ConflictException(
                    string.Format(AppConstants.EventAlreadyExists, request.EventRequestId));

            var newEvent = BuildEvent(request, eventRequest);
            newEvent.Categories = EventCategoryFactory.Generate(eventRequest.Gender, eventRequest.Format);

            await _eventRepo.AddAsync(newEvent);
            await _eventRepo.SaveChangesAsync();

            var created = await _eventRepo.GetByIdWithDetailsAsync(newEvent.Id);
            return _mapper.Map<EventResponse>(created!);
        }

        public async Task<EventResponse> PatchEventAsync(int eventId, PatchEventRequest request)
        {
            var entity = await _eventRepo.GetByIdWithDetailsAsync(eventId)
                ?? throw new NotFoundException(string.Format(AppConstants.EventNotFound, eventId));

            ValidateEventEditable(entity);

            var action = request.Action.Trim().ToLower();

            if (action == "cancel")
            {
                entity.Status    = EventStatus.Cancelled;
                entity.UpdatedAt = DateTime.UtcNow;
            }
            else if (action == "publish")
            {
                if (entity.Status != EventStatus.Upcoming)
                    throw new BadRequestException("Only Upcoming events can be published.");
                entity.Status    = EventStatus.Open;
                entity.UpdatedAt = DateTime.UtcNow;
            }
            else if (action == "update")
            {
                if (request.Name != null)
                    entity.Name = request.Name.Trim();

                if (request.Description != null)
                    entity.Description = request.Description.Trim();

                if (request.MaxParticipantsCount.HasValue)
                    entity.MaxParticipantsCount = request.MaxParticipantsCount.Value;

                if (request.RegistrationDeadline.HasValue)
                {
                    if (request.RegistrationDeadline.Value >= entity.StartDate)
                        throw new BadRequestException(AppConstants.RegistrationDeadlineInvalid);
                    entity.RegistrationDeadline = request.RegistrationDeadline.Value;
                    if (entity.Status == EventStatus.Cancelled)
                        entity.Status = EventStatus.Open;
                }

                entity.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                throw new BadRequestException($"Invalid action '{request.Action}'. Use 'update', 'publish' or 'cancel'.");
            }

            _eventRepo.Update(entity);
            await _eventRepo.SaveChangesAsync();

            return _mapper.Map<EventResponse>(entity);
        }

        public async Task<IEnumerable<EventCategoryResponse>> GetCategoriesByEventIdAsync(int eventId)
        {
            var entity = await _eventRepo.GetByIdWithDetailsAsync(eventId)
                ?? throw new NotFoundException(string.Format(AppConstants.EventNotFound, eventId));
            return _mapper.Map<IEnumerable<EventCategoryResponse>>(entity.Categories);
        }

        public async Task<EventResponse> AssignOrganizerAsync(int eventId, int organizerId)
        {
            var entity = await _eventRepo.GetByIdWithDetailsAsync(eventId)
                ?? throw new NotFoundException(string.Format(AppConstants.EventNotFound, eventId));

            ValidateEventEditable(entity);

            var organizer = await _userRepo.GetByIdWithRoleAsync(organizerId)
                ?? throw new NotFoundException(string.Format(AppConstants.UserNotFound, organizerId));

            ValidateOrganizer(organizer);

            entity.OrganizerId = organizer.Id;
            entity.Organizer   = organizer;
            entity.UpdatedAt   = DateTime.UtcNow;

            _eventRepo.Update(entity);
            await _eventRepo.SaveChangesAsync();

            return _mapper.Map<EventResponse>(entity);
        }

        private static void ValidateEventCreation(EventRequest eventRequest, CreateEventRequest request)
        {
            if (eventRequest.Status != RequestStatus.Approved)
                throw new UnprocessableEntityException(
                    string.Format(AppConstants.EventRequestNotApproved, eventRequest.Status));
            if (request.RegistrationDeadline >= eventRequest.StartDate)
                throw new BadRequestException(AppConstants.RegistrationDeadlineInvalid);
        }

        private static void ValidateEventEditable(Event entity)
        {
            if (entity.Status == EventStatus.Completed)
                throw new UnprocessableEntityException(AppConstants.EventCompleted);
            if (entity.Status == EventStatus.Cancelled)
                throw new UnprocessableEntityException(AppConstants.EventCancelled);
        }

        private static void ValidateOrganizer(User organizer)
        {
            if (!organizer.IsActive)
                throw new UnprocessableEntityException(
                    string.Format(AppConstants.UserInactive, organizer.FullName));
            if (organizer.RoleId != AppConstants.OrganizerRoleId)
                throw new UnprocessableEntityException(
                    string.Format(AppConstants.UserNotOrganizer, organizer.FullName));
        }

        private static Event BuildEvent(CreateEventRequest request, EventRequest eventRequest) => new()
        {
            EventRequestId       = request.EventRequestId,
            Name                 = !string.IsNullOrWhiteSpace(request.Name) ? request.Name : eventRequest.EventName,
            SportId              = eventRequest.SportId,
            StartDate            = eventRequest.StartDate,
            EndDate              = eventRequest.EndDate,
            EventVenue           = eventRequest.RequestedVenue,
            RegistrationDeadline = request.RegistrationDeadline,
            MaxParticipantsCount = request.MaxParticipantsCount,
            Description          = request.Description,
            Status               = EventStatus.Upcoming,
            OrganizerId          = eventRequest.AdminId,
            TournamentType       = TournamentType.Knockout,
            CreatedAt            = DateTime.UtcNow
        };
    }
}