using AuthCore.Application.Models;
using AuthCore.Domain.Shared;
using System.Net;
using System.Text.Json;

namespace AuthCore.Api.Configurations.Extensions
{
    public static class Exceptions
    {
        /// <summary>
        /// Registra o middleware de tratamento global de exceções no pipeline HTTP.
        /// </summary>
        /// <param name="app">Instância do <see cref="WebApplication"/> utilizada para configuração do pipeline.</param>
        public static void UseExceptionsHandling(this WebApplication app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }

        /// <summary>
        /// Middleware responsável por capturar exceções não tratadas durante o processamento das requisições
        /// e gerar respostas padronizadas no formato JSON.
        /// </summary>
        private sealed class ExceptionMiddleware
        {
            private readonly RequestDelegate _next;
            private readonly ILogger<ExceptionMiddleware> _logger;
            private readonly IHostEnvironment _env;

            /// <summary>
            /// Inicializa uma nova instância do <see cref="ExceptionMiddleware"/>.
            /// </summary>
            /// <param name="next">Delegado que representa o próximo middleware na cadeia de execução.</param>
            /// <param name="logger">Instância do logger para registrar erros.</param>
            /// <param name="env">Instância do ambiente de hospedagem atual.</param>
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
            /// Captura exceções lançadas durante o processamento da requisição e retorna
            /// uma resposta padronizada com código de status, título e mensagens de erro.
            /// </summary>
            /// <param name="context">Contexto atual da requisição HTTP.</param>
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

                    // Exceções de domínio com status e título customizados
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
                        title = "Erro interno do servidor";
                        errors = new[] { "Ocorreu um erro interno no servidor." };
                    }

                    // Inclui detalhes técnicos apenas em ambiente de desenvolvimento
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
                    }, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = (int)status;
                    await context.Response.WriteAsync(json);
                }
            }
        }
    }
}
