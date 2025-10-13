using AuthCore.Application.Models;
using AuthCore.Domain.Commons.Exceptions;
using System.Net;
using System.Text.Json;

namespace AuthCore.Api.Configurations
{
    public static class ExceptionHandlingExtensions
    {
        public static void UseExceptionHandling(this WebApplication app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }

        private sealed class ExceptionMiddleware
        {
            private readonly RequestDelegate _next;
            private readonly ILogger<ExceptionMiddleware> _logger;
            private readonly IHostEnvironment _env;

            public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
            {
                _next = next;
                _logger = logger;
                _env = env;
            }

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

                    // Se for exceção de domínio, usa as propriedades dela
                    if (ex is DomainException domainEx)
                    {
                        status = domainEx.StatusCode;
                        title = domainEx.Title;
                        errors = domainEx.Errors;
                    }
                    else
                    {
                        // Fallback genérico
                        status = HttpStatusCode.InternalServerError;
                        title = "Erro interno no servidor";
                        errors = new[] { "Ocorreu um erro interno no servidor." };
                    }

                    object? details = _env.IsDevelopment()
                        ? new
                        {
                            ex.Message,
                            ex.StackTrace,
                            Inner = ex.InnerException?.Message
                        }
                        : null;

                    var response = Response<object>.Error(errors, title, status);

                    var json = JsonSerializer.Serialize(new
                    {
                        response.StatusCode,
                        response.Title,
                        response.Errors,
                        Details = details
                    }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)status;
                    await context.Response.WriteAsync(json);
                }
            }
        }
    }
}
