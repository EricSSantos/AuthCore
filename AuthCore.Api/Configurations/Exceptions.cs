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

            public ExceptionMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public async Task InvokeAsync(HttpContext context)
            {
                try
                {
                    await _next(context);
                }
                catch
                {
                    await WriteErrors(
                        context,
                        new[] { "Ocorreu um erro interno." },
                        HttpStatusCode.InternalServerError,
                        "Erro interno no servidor");
                }
            }

            private static async Task WriteErrors(
                HttpContext context,
                IReadOnlyCollection<string> errors,
                HttpStatusCode status,
                string title)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)status;

                var response = new
                {
                    statusCode = (int)status,
                    title,
                    errors
                };

                var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await context.Response.WriteAsync(json);
            }
        }
    }
}
