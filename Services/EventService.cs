using SportsManagementApp.DTOs.Request;
using SportsManagementApp.DTOs.Response;
using SportsManagementApp.Entities;
using SportsManagementApp.Enums;
using SportsManagementApp.Repositories.Interfaces;
using SportsManagementApp.Services.Interfaces;
using SportsManagementApp.StringConstants;

namespace SportsManagementApp.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository         _eventRepo;
        private readonly IEventRequestRepository  _requestRepo;
        private readonly IUserRepository          _userRepo;

        private const int OrganizerRoleId = 2;

        public EventService(
            IEventRepository        eventRepo,
            IEventRequestRepository requestRepo,
            IUserRepository         userRepo)
        {
            _eventRepo   = eventRepo;
            _requestRepo = requestRepo;
            _userRepo    = userRepo;
        }

        public async Task<ServiceResult<EventResponse>> CreateEventFromRequestAsync(
            CreateEventRequest request)
        {
            var eventRequest = await _requestRepo.GetByIdAsync(request.EventRequestId);

            if (eventRequest is null)
                return ServiceResult<EventResponse>.NotFound(
                    string.Format(AppConstants.EventRequestNotFound, request.EventRequestId));

            if (eventRequest.Status != RequestStatus.Approved)
                return ServiceResult<EventResponse>.UnprocessableEntity(
                    string.Format(AppConstants.EventRequestNotApproved, eventRequest.Status));

            bool duplicate = await _eventRepo.ExistsByNameSportDateAsync(
                eventRequest.EventName, eventRequest.SportId, eventRequest.StartDate);

            if (duplicate)
                return ServiceResult<EventResponse>.Conflict(
                    string.Format(AppConstants.EventAlreadyExists, eventRequest.EventName, eventRequest.StartDate));

            if (request.RegistrationDeadline >= eventRequest.StartDate)
                return ServiceResult<EventResponse>.BadRequest(AppConstants.RegistrationDeadlineInvalid);

            var newEvent = new Event
            {
                Name                 = eventRequest.EventName,
                SportId              = eventRequest.SportId,
                StartDate            = eventRequest.StartDate,
                EndDate              = eventRequest.EndDate,
                EventVenue           = request.EventVenue,
                RegistrationDeadline = request.RegistrationDeadline,
                Description          = request.Description,
                Status               = EventStatus.Upcoming,
                OrganizerId          = eventRequest.AdminId,
                CreatedAt            = DateTime.UtcNow
            };

            await _eventRepo.AddAsync(newEvent);
            await _eventRepo.SaveChangesAsync();

            var created = await _eventRepo.GetByIdAsync(newEvent.Id, includeCategories: true);
            return ServiceResult<EventResponse>.Success(MapToResponse(created!), 201);
        }

        public async Task<ServiceResult<EventResponse>> AssignOrganizerAsync(
            int eventId, AssignOrganizerRequest request)
        {
            var eventEntity = await _eventRepo.GetByIdAsync(eventId, includeCategories: true);

            if (eventEntity is null)
                return ServiceResult<EventResponse>.NotFound(
                    string.Format(AppConstants.EventNotFound, eventId));

            if (eventEntity.Status is EventStatus.Completed or EventStatus.Cancelled)
                return ServiceResult<EventResponse>.UnprocessableEntity(
                    string.Format(AppConstants.EventNotAssignable, eventEntity.Status));

            var organizer = await _userRepo.GetByIdWithRoleAsync(request.OrganizerId);

            if (organizer is null)
                return ServiceResult<EventResponse>.NotFound(
                    string.Format(AppConstants.UserNotFound, request.OrganizerId));

            if (!organizer.IsActive)
                return ServiceResult<EventResponse>.UnprocessableEntity(
                    string.Format(AppConstants.UserInactive, organizer.FullName));

            if (organizer.RoleId != OrganizerRoleId)
                return ServiceResult<EventResponse>.UnprocessableEntity(
                    string.Format(AppConstants.UserNotOrganizer, organizer.FullName));

            eventEntity.OrganizerId = organizer.Id;
            eventEntity.Organizer   = organizer;
            eventEntity.UpdatedAt   = DateTime.UtcNow;

            _eventRepo.Update(eventEntity);
            await _eventRepo.SaveChangesAsync();

            return ServiceResult<EventResponse>.Success(MapToResponse(eventEntity));
        }

        public async Task<ServiceResult<EventResponse>> ConfigureEventAsync(
            int eventId, EventConfigurationRequest request)
        {
            var eventEntity = await _eventRepo.GetByIdAsync(eventId, includeCategories: true);

            if (eventEntity is null)
                return ServiceResult<EventResponse>.NotFound(
                    string.Format(AppConstants.EventNotFound, eventId));

            if (eventEntity.Status is EventStatus.Completed or EventStatus.Cancelled)
                return ServiceResult<EventResponse>.UnprocessableEntity(
                    string.Format(AppConstants.EventNotConfigurable, eventEntity.Status));

            if (request.Rules is not null)
            {
                if (request.Rules.StartDate >= request.Rules.EndDate)
                    return ServiceResult<EventResponse>.BadRequest(AppConstants.StartDateBeforeEndDate);

                if (request.Rules.RegistrationDeadline >= request.Rules.StartDate)
                    return ServiceResult<EventResponse>.BadRequest(AppConstants.DeadlineBeforeStartDate);

                eventEntity.StartDate            = request.Rules.StartDate;
                eventEntity.EndDate              = request.Rules.EndDate;
                eventEntity.RegistrationDeadline = request.Rules.RegistrationDeadline;
                eventEntity.EventVenue           = request.Rules.EventVenue;
                eventEntity.Description          = request.Rules.Description;
            }

            if (request.Categories.Any())
            {
                var duplicate = request.Categories
                    .GroupBy(c => new { c.Format, c.Gender })
                    .FirstOrDefault(g => g.Count() > 1);

                if (duplicate is not null)
                    return ServiceResult<EventResponse>.BadRequest(
                        string.Format(AppConstants.DuplicateCategory, duplicate.Key.Format, duplicate.Key.Gender));

                foreach (var cat in request.Categories)
                {
                    bool exists = eventEntity.Categories.Any(
                        c => c.Format == cat.Format && c.Gender == cat.Gender);

                    if (exists) continue;

                    eventEntity.Categories.Add(new EventCategory
                    {
                        Format         = cat.Format,
                        Gender         = cat.Gender,
                        TournamentType = cat.TournamentType,
                        Status         = CategoryStatus.Active,
                        EventId        = eventEntity.Id,
                        CreatedAt      = DateTime.UtcNow
                    });
                }
            }

            if (request.TeamLimits is not null)
            {
                if (request.TeamLimits.MaxTeamsPerCategory < 2)
                    return ServiceResult<EventResponse>.BadRequest(AppConstants.MaxTeamsMinError);

                if (request.TeamLimits.MaxMembersPerTeam < 1)
                    return ServiceResult<EventResponse>.BadRequest(AppConstants.MaxMembersMinError);
            }

            eventEntity.UpdatedAt = DateTime.UtcNow;

            _eventRepo.Update(eventEntity);
            await _eventRepo.SaveChangesAsync();

            return ServiceResult<EventResponse>.Success(MapToResponse(eventEntity));
        }

        public async Task<ServiceResult<EventResponse>> GetByIdAsync(int eventId)
        {
            var eventEntity = await _eventRepo.GetByIdAsync(eventId, includeCategories: true);

            if (eventEntity is null)
                return ServiceResult<EventResponse>.NotFound(
                    string.Format(AppConstants.EventNotFound, eventId));

            return ServiceResult<EventResponse>.Success(MapToResponse(eventEntity));
        }

        public async Task<ServiceResult<List<EventResponse>>> GetAllAsync()
        {
            var events = await _eventRepo.GetAllAsync();
            return ServiceResult<List<EventResponse>>.Success(
                events.Select(MapToResponse).ToList());
        }

        private static EventResponse MapToResponse(Event e) => new()
        {
            Id                   = e.Id,
            Name                 = e.Name,
            SportName            = e.Sport?.Name ?? string.Empty,
            StartDate            = e.StartDate,
            EndDate              = e.EndDate,
            EventVenue           = e.EventVenue,
            RegistrationDeadline = e.RegistrationDeadline,
            OrganizerName        = e.Organizer?.FullName,
            Status               = e.Status.ToString(),
            Description          = e.Description,
            CreatedAt            = e.CreatedAt,
            UpdatedAt            = e.UpdatedAt,
            Categories           = e.Categories.Select(c => new EventCategoryResponse
            {
                Id             = c.Id,
                Format         = c.Format.ToString(),
                Gender         = c.Gender.ToString(),
                TournamentType = c.TournamentType.ToString(),
                Status         = c.Status.ToString(),
                CreatedAt      = c.CreatedAt
            }).ToList()
        };
    }
}