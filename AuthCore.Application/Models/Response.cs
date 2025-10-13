using System.Net;

namespace AuthCore.Application.Models
{
    public class Response<T>
    {
        public HttpStatusCode StatusCode { get; init; }
        public string Title { get; init; } = string.Empty;
        public T? Data { get; init; }
        public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();

        protected Response() { }

        public static Response<T> Success(T data, string title, HttpStatusCode statusCode)
        {
            return new Response<T>
            {
                StatusCode = statusCode,
                Title = title,
                Data = data
            };
        }

        public static Response<T> Error(IEnumerable<string> errors, string title, HttpStatusCode statusCode)
        {
            return new Response<T>
            {
                StatusCode = statusCode,
                Title = title,
                Errors = errors.ToList()
            };
        }
    }
}
