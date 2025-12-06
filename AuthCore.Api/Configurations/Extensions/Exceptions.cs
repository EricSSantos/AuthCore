using AuthCore.Application.Models;
using AuthCore.Domain.Core.Exceptions;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AuthCore.Api.Configurations.Extensions
{
    /// <summary>
    /// Configura o tratamento global de exceções da aplicação.
    /// </summary>
    public static class Exceptions
    {
        /// <summary>
        /// Ativa o middleware global de erros.
        /// </summary>
        /// <param name="app">Instância atual da aplicação.</param>
        public static void UseExceptionsHandling(this WebApplication app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }

        /// <summary>
        /// Middleware que captura exceções e retorna respostas padronizadas.
        /// </summary>
        private sealed class ExceptionMiddleware
        {
            private readonly RequestDelegate _next;
            private readonly ILogger<ExceptionMiddleware> _logger;
            private readonly IHostEnvironment _env;

            /// <summary>
            /// Inicializa o middleware de exceções.
            /// </summary>
            /// <param name="next">Próximo middleware do pipeline.</param>
            /// <param name="logger">Logger para registrar erros.</param>
            /// <param name="env">Ambiente de execução.</param>
            public ExceptionMiddleware(
                RequestDelegate next,
                ILogger<ExceptionMiddleware> logger,
                IHostEnvironment env)
            {
                _next = next;
                _logger = logger;
                _env = env;
            }

            /// <summary>
            /// Processa a requisição e trata erros não capturados.
            /// </summary>
            /// <param name="context">Contexto HTTP atual.</param>
            public async Task InvokeAsync(HttpContext context)
            {
                try
                {
                    await _next(context);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro não tratado: {Message}", ex.Message);

                    HttpStatusCode status;
                    string title;
                    IReadOnlyCollection<string> errors;

                    if (ex is DomainException domainEx)
                    {
                        status = domainEx.StatusCode;
                        title = domainEx.Title;
                        errors = domainEx.Errors;
                    }
                    else
                    {
                        status = HttpStatusCode.InternalServerError;
                        title = "Erro interno do servidor";
                        errors = new[] { "Ocorreu um erro interno no servidor." };
                    }

                    object? details = _env.IsDevelopment()
                        ? new
                        {
                            ex.StackTrace,
                            Inner = ex.InnerException?.Message
                        }
                        : null;

                    var response = ApiResponse<object>.Error(errors, title, status);

                    var json = JsonSerializer.Serialize(new
                    {
                        response.StatusCode,
                        response.Title,
                        response.Errors,
                        Details = details
                    },
                    new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                    });

                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)status;
                    await context.Response.WriteAsync(json);
                }
            }
        }
    }
}
