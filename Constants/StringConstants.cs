namespace SportsManagementApp.StringConstants
{
    public static class AppConstants
    {
        public const string AdminRole             = "Admin";
        public const string UserRole              = "User";
        public const string ApiVersion            = "v1";
        public const string ReactNativeCorsPolicy = "ReactNativePolicy";
        public const string SwaggerTitle          = "Sports Management API";
        public const string SwaggerDescription    = "Backend API for the Sports Management React Native app.";
        public const string EventCreated          = "Event created successfully.";
        public const string EventConfigured       = "Event configured successfully.";
        public const string OrganizerCreated      = "Organizer assigned successfully.";
        public const string RequiredValueMissing  = "A required value was missing.";
        public const string UnauthorizedAccess    = "Unauthorized access.";
        public const string EventRequestIdRequired      = "EventRequestId is required.";
        public const string EventVenueRequired          = "Event venue is required.";
        public const string EventVenueTooLong           = "Venue cannot exceed 200 characters.";
        public const string RegistrationDeadlineRequired = "Registration deadline is required.";
        public const string DescriptionTooLong          = "Description cannot exceed 1000 characters.";
        public const string MaxTeamsRangeError   = "MaxTeamsPerCategory must be between 2 and 256.";
        public const string MaxMembersRangeError = "MaxMembersPerTeam must be between 1 and 50.";
        public const string EventRequestNotFound        = "Event request #{0} not found.";
        public const string EventRequestNotApproved     = "Event request must be Approved to create an event. Current status: {0}.";
        public const string EventAlreadyExists          = "An event for '{0}' on {1} already exists.";
        public const string RegistrationDeadlineInvalid = "Registration deadline must be before the event start date.";
        public const string EventNotFound               = "Event #{0} not found.";
        public const string EventNotConfigurable        = "Cannot configure a {0} event.";
        public const string EventNotAssignable          = "Cannot assign an organizer to a {0} event.";
        public const string StartDateBeforeEndDate      = "Start date must be before end date.";
        public const string DeadlineBeforeStartDate     = "Registration deadline must be before the start date.";
        public const string DuplicateCategory           = "Duplicate category submitted: {0} / {1}.";
        public const string MaxTeamsMinError            = "MaxTeamsPerCategory must be at least 2.";
        public const string MaxMembersMinError          = "MaxMembersPerTeam must be at least 1.";
        public const string UserNotFound           = "User #{0} not found.";
        public const string UserInactive           = "User '{0}' is inactive and cannot be assigned as organizer.";
        public const string UserNotOrganizer       = "User '{0}' does not hold the Organizer role.";
        public const string UnexpectedError = "An unexpected error occurred.";
        public const string Unauthorized    = "Unauthorized.";
        public const string UnhandledExceptionLog = "Unhandled exception on {Method} {Path}";
        public const string SwaggerEndpoint    = "/swagger/v1/swagger.json";
        public const string SwaggerDisplayName = "Sports Management API v1";
    }
}