using System.Net;

namespace AuthCore.Application.Models
{
    public class ApiResponse<T>
    {
        public HttpStatusCode StatusCode { get; init; }
        public string Title { get; init; } = string.Empty;
        public T? Data { get; init; }
        public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();

        protected ApiResponse() { }

        public static ApiResponse<T> Success(T data, string title, HttpStatusCode statusCode)
        {
            return new ApiResponse<T>
            {
                StatusCode = statusCode,
                Title = title,
                Data = data
            };
        }

        public static ApiResponse<T> Error(IEnumerable<string> errors, string title, HttpStatusCode statusCode)
        {
            return new ApiResponse<T>
            {
                StatusCode = statusCode,
                Title = title,
                Errors = errors.ToList()
            };
        }
    }
}
