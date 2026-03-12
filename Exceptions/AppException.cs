using System.Net;

namespace SportsManagementApp.Exceptions
{
    public abstract class AppException(string message, HttpStatusCode statusCode) : Exception(message)
    {
        public HttpStatusCode StatusCode { get; } = statusCode;
    }

    public class NotFoundException(string message) : AppException(message, HttpStatusCode.NotFound) { }

    public class BadRequestException(string message) : AppException(message, HttpStatusCode.BadRequest) { }

    public class UnauthorizedException(string message) : AppException(message, HttpStatusCode.Unauthorized) { }

    public class ConflictException(string message) : AppException(message, HttpStatusCode.Conflict) { }
}
