using AutoMapper;
using SportsManagementApp.DTOs.Request;
using SportsManagementApp.DTOs.Response;
using SportsManagementApp.Entities;
using SportsManagementApp.Enums;
using SportsManagementApp.Exceptions;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;
using SportsManagementApp.StringConstants;

namespace SportsManagementApp.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository        _eventRepo;
        private readonly IEventRequestRepository _requestRepo;
        private readonly IUserRepository         _userRepo;
        private readonly IMapper                 _mapper;

        public EventService(
            IEventRepository        eventRepo,
            IEventRequestRepository requestRepo,
            IUserRepository         userRepo,
            IMapper                 mapper)
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

            var events = await _eventRepo.GetAllWithCategoriesAsync(filter.Status, filter.Name, filter.SportId);
            return _mapper.Map<IEnumerable<EventResponse>>(events);
        }

        public async Task<EventResponse> GetByIdAsync(int eventId)
        {
            var eventEntity = await _eventRepo.GetByIdWithDetailsAsync(eventId)
                ?? throw new NotFoundException(string.Format(AppConstants.EventNotFound, eventId));

            return _mapper.Map<EventResponse>(eventEntity);
        }

        public async Task<EventRequestPreFillResponse> GetEventRequestForPreFillAsync(int requestId)
        {
            var eventRequest = await _requestRepo.GetByIdWithDetailsAsync(requestId)
                ?? throw new NotFoundException(string.Format(AppConstants.EventRequestNotFound, requestId));

            if (eventRequest.Status != RequestStatus.Approved)
                throw new UnprocessableEntityException(
                    string.Format(AppConstants.EventRequestNotApproved, eventRequest.Status));

            bool eventAlreadyCreated = await _eventRepo.ExistsByRequestIdAsync(requestId);

            return new EventRequestPreFillResponse
            {
                Id                   = eventRequest.Id,
                SportName            = eventRequest.Sport?.Name ?? string.Empty,
                Gender               = eventRequest.Gender.ToString(),
                Format               = eventRequest.Format.ToString(),
                RequestedVenue       = eventRequest.RequestedVenue,
                StartDate            = eventRequest.StartDate,
                EndDate              = eventRequest.EndDate,
                Status               = eventRequest.Status.ToString(),
                IsEventAlreadyCreated = eventAlreadyCreated,

                Name                 = eventRequest.EventName,
                Description          = null,
                RegistrationDeadline = null,
            };
        }

        public async Task<EventResponse> CreateEventFromRequestAsync(CreateEventRequest request)
        {
            var eventRequest = await _requestRepo.GetByIdWithDetailsAsync(request.EventRequestId)
                ?? throw new NotFoundException(string.Format(AppConstants.EventRequestNotFound, request.EventRequestId));

            if (eventRequest.Status != RequestStatus.Approved)
                throw new UnprocessableEntityException(
                    string.Format(AppConstants.EventRequestNotApproved, eventRequest.Status));

            if (await _eventRepo.ExistsByRequestIdAsync(request.EventRequestId))
                throw new ConflictException(string.Format(AppConstants.EventAlreadyExists, request.EventRequestId));

            if (request.RegistrationDeadline >= eventRequest.StartDate)
                throw new BadRequestException(AppConstants.RegistrationDeadlineInvalid);

            var newEvent = new Event
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

            newEvent.Categories = GenerateCategories(eventRequest.Gender, eventRequest.Format);

            await _eventRepo.AddAsync(newEvent);
            await _eventRepo.SaveChangesAsync();

            var created = await _eventRepo.GetByIdWithDetailsAsync(newEvent.Id);
            return _mapper.Map<EventResponse>(created!);
        }

        public async Task<IEnumerable<EventCategoryResponse>> GetCategoriesByEventIdAsync(int eventId)
        {
            var eventEntity = await _eventRepo.GetByIdWithDetailsAsync(eventId)
                ?? throw new NotFoundException(string.Format(AppConstants.EventNotFound, eventId));

            return _mapper.Map<IEnumerable<EventCategoryResponse>>(eventEntity.Categories);
        }

        public async Task<EventResponse> AssignOrganizerAsync(int eventId, AssignOrganizerRequest request)
        {
            var eventEntity = await _eventRepo.GetByIdWithDetailsAsync(eventId)
                ?? throw new NotFoundException(string.Format(AppConstants.EventNotFound, eventId));

            if (eventEntity.Status == EventStatus.Completed)
                throw new UnprocessableEntityException(AppConstants.EventCompleted);

            if (eventEntity.Status == EventStatus.Cancelled)
                throw new UnprocessableEntityException(AppConstants.EventCancelled);

            var organizer = await _userRepo.GetByIdWithRoleAsync(request.OrganizerId)
                ?? throw new NotFoundException(string.Format(AppConstants.UserNotFound, request.OrganizerId));

            if (!organizer.IsActive)
                throw new UnprocessableEntityException(string.Format(AppConstants.UserInactive, organizer.FullName));

            if (organizer.RoleId != AppConstants.OrganizerRoleId)
                throw new UnprocessableEntityException(string.Format(AppConstants.UserNotOrganizer, organizer.FullName));

            eventEntity.OrganizerId = organizer.Id;
            eventEntity.Organizer   = organizer;
            eventEntity.UpdatedAt   = DateTime.UtcNow;

            _eventRepo.Update(eventEntity);
            await _eventRepo.SaveChangesAsync();

            return _mapper.Map<EventResponse>(eventEntity);
        }

        private static List<EventCategory> GenerateCategories(GenderType gender, MatchFormat format)
        {
            var categories = new List<EventCategory>();
            var now        = DateTime.UtcNow;

            if (gender == GenderType.Mixed)
            {
                categories.Add(MakeCategory(GenderType.Mixed, MatchFormat.Singles, now));
                return categories;
            }

            var genders = gender switch
            {
                GenderType.Male   => new[] { GenderType.Male },
                GenderType.Female => new[] { GenderType.Female },
                GenderType.Both   => new[] { GenderType.Male, GenderType.Female },
                _                 => Array.Empty<GenderType>()
            };

            var formats = format switch
            {
                MatchFormat.Singles => new[] { MatchFormat.Singles },
                MatchFormat.Doubles => new[] { MatchFormat.Doubles },
                MatchFormat.Both    => new[] { MatchFormat.Singles, MatchFormat.Doubles },
                _                   => Array.Empty<MatchFormat>()
            };

            foreach (var g in genders)
                foreach (var f in formats)
                    categories.Add(MakeCategory(g, f, now));

            return categories;
        }

        private static EventCategory MakeCategory(GenderType gender, MatchFormat format, DateTime now) => new()
        {
            Gender    = gender,
            Format    = format,
            Status    = CategoryStatus.Active,
            CreatedAt = now
        };
    }
}