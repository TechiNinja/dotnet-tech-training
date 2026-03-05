using SportsManagementApp.Exceptions;
using SportsManagementApp.StringConstants;
using System.Net;

namespace SportsManagementApp.Middleware
{   
    public static class ExceptionStatusMapper
    {
        public static (HttpStatusCode StatusCode, string Message) Map(Exception ex) => ex switch
        {
            NotFoundException            => (HttpStatusCode.NotFound,            ex.Message),
            ConflictException            => (HttpStatusCode.Conflict,            ex.Message),
            BadRequestException          => (HttpStatusCode.BadRequest,          ex.Message),
            UnprocessableEntityException => (HttpStatusCode.UnprocessableEntity, ex.Message),
            UnauthorizedAccessException  => (HttpStatusCode.Unauthorized,        AppConstants.UnauthorizedAccess),
            _                            => (HttpStatusCode.InternalServerError, AppConstants.UnexpectedError)
        };
    }
}
