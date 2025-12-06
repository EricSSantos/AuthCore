using System.Net;

namespace AuthCore.Application.Models
{
    /// <summary>
    /// Representa uma resposta padrão da API.
    /// </summary>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Define o status HTTP da resposta.
        /// </summary>
        public HttpStatusCode StatusCode { get; init; }

        /// <summary>
        /// Define um título descritivo da resposta.
        /// </summary>
        public string Title { get; init; } = string.Empty;

        /// <summary>
        /// Contém os dados retornados pela operação.
        /// </summary>
        public T? Data { get; init; }

        /// <summary>
        /// Lista erros associados à resposta.
        /// </summary>
        public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();

        protected ApiResponse() { }

        /// <summary>
        /// Cria uma resposta de sucesso.
        /// </summary>
        /// <param name="data">Dados retornados.</param>
        /// <param name="title">Título da resposta.</param>
        /// <param name="statusCode">Status HTTP.</param>
        /// <returns>Instância representando sucesso.</returns>
        public static ApiResponse<T> Success(T data, string title, HttpStatusCode statusCode)
        {
            return new ApiResponse<T>
            {
                StatusCode = statusCode,
                Title = title,
                Data = data
            };
        }

        /// <summary>
        /// Cria uma resposta de erro.
        /// </summary>
        /// <param name="errors">Lista de mensagens de erro.</param>
        /// <param name="title">Título da resposta.</param>
        /// <param name="statusCode">Status HTTP.</param>
        /// <returns>Instância representando erro.</returns>
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
