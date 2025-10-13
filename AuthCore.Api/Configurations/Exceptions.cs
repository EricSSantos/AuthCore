using AuthCore.Application.Models;
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

                    var errors = new List<string> { "Ocorreu um erro interno no servidor." };
                    var title = "Erro interno no servidor";
                    var status = HttpStatusCode.InternalServerError;
                    object? details = null;

                    // Em ambiente de desenvolvimento exibe detalhes da exceção
                    if (_env.IsDevelopment())
                    {
                        details = new
                        {
                            ex.Message,
                            ex.StackTrace,
                            Inner = ex.InnerException?.Message
                        };
                    }

                    var response = Response<object>.Error(errors, title, status);

                    var json = JsonSerializer.Serialize(new
                    {
                        response.StatusCode,
                        response.Title,
                        response.Errors,
                        Details = details
                    },
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)status;
                    await context.Response.WriteAsync(json);
                }
            }
        }
    }
}
