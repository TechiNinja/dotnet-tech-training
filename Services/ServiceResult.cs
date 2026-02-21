namespace SportsManagementApp.Services
{
    public class ServiceResult<T>
    {
        public bool IsSuccess { get; private set; }
        public T? Data { get; private set; }
        public string? Error { get; private set; }
        public int StatusCode { get; private set; }

        private ServiceResult() { }

        public static ServiceResult<T> Success(T data, int statusCode = 200) =>
            new() { IsSuccess = true, Data = data, StatusCode = statusCode };

        public static ServiceResult<T> NotFound(string message) =>
            new() { IsSuccess = false, Error = message, StatusCode = 404 };

        public static ServiceResult<T> Conflict(string message) =>
            new() { IsSuccess = false, Error = message, StatusCode = 409 };

        public static ServiceResult<T> BadRequest(string message) =>
            new() { IsSuccess = false, Error = message, StatusCode = 400 };

        public static ServiceResult<T> UnprocessableEntity(string message) =>
            new() { IsSuccess = false, Error = message, StatusCode = 422 };
    }
}