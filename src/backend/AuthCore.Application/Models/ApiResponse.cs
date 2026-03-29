using System.Net;

namespace AuthCore.Application.Models
{
    /// <summary>Representa uma resposta padrão da API.</summary>
    public class ApiResponse<T>
    {
        public HttpStatusCode StatusCode { get; set; }

        public string Title { get; set; } = string.Empty;

        public T? Data { get; set; }

        public IReadOnlyList<string> Errors { get; set; } = Array.Empty<string>();

        protected ApiResponse() { }

        /// <summary>Operação para criar uma resposta de sucesso.</summary>
        /// <param name="data">Dados retornados.</param>
        /// <param name="title">Título da resposta.</param>
        /// <param name="statusCode">Status HTTP.</param>
        public static ApiResponse<T> Success(T data, string title, HttpStatusCode statusCode)
        {
            return new ApiResponse<T>
            {
                StatusCode = statusCode,
                Title = title,
                Data = data
            };
        }

        /// <summary>Operação para criar uma resposta de erro.</summary>
        /// <param name="errors">Lista de mensagens de erro.</param>
        /// <param name="title">Título da resposta.</param>
        /// <param name="statusCode">Status HTTP.</param>
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
