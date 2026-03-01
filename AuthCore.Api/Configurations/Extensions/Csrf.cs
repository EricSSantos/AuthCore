using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace AuthCore.Api.Configurations.Extensions
{
    /// <summary>Configura proteção CSRF por cookie de submissão dupla.</summary>
    public static class Csrf
    {
        /// <summary>Operação para adicionar middleware de proteção CSRF.</summary>
        /// <param name="app">Instância da aplicação.</param>
        public static void UseCsrfProtection(this WebApplication app)
        {
            app.UseMiddleware<CsrfMiddleware>();
        }

        /// <summary>Representa middleware responsável pela validação CSRF das requisições.</summary>
        private sealed class CsrfMiddleware
        {
            /// <summary>Headers aceitos para transporte do token CSRF.</summary>
            private static readonly string[] CSRF_HEADERS = { "X-CSRF", "X-CSRF-TOKEN", "X-XSRF-TOKEN" };

            /// <summary>Métodos HTTP considerados seguros para bypass da validação CSRF.</summary>
            private static readonly HashSet<string> SAFE_METHODS = new(StringComparer.OrdinalIgnoreCase)
            {
                HttpMethods.Get,
                HttpMethods.Head,
                HttpMethods.Options,
                HttpMethods.Trace
            };

            /// <summary>Próximo middleware do pipeline HTTP.</summary>
            private readonly RequestDelegate _next;

            /// <summary>Configurações de segurança utilizadas na validação CSRF.</summary>
            private readonly SecuritySettings _settings;

            /// <summary>Operação para criar instância do middleware de proteção CSRF.</summary>
            /// <param name="next">Próximo middleware do pipeline.</param>
            /// <param name="settings">Configurações de segurança.</param>
            public CsrfMiddleware(RequestDelegate next, IOptions<SecuritySettings> settings)
            {
                _next = next;
                _settings = settings.Value;
            }

            /// <summary>Operação para validar proteção CSRF na requisição atual.</summary>
            /// <param name="context">Contexto HTTP da requisição.</param>
            public async Task InvokeAsync(HttpContext context)
            {
                if (SAFE_METHODS.Contains(context.Request.Method))
                {
                    await _next(context);
                    return;
                }

                var path = context.Request.Path.Value ?? string.Empty;
                if (path.Equals("/api/v1/auth/sign-in", StringComparison.OrdinalIgnoreCase))
                {
                    await _next(context);
                    return;
                }

                var hasAuthCookies = context.Request.Cookies.ContainsKey(_settings.Cookies.SessionKey)
                    || context.Request.Cookies.ContainsKey(_settings.Cookies.AccessTokenKey);

                var endpointRequiresAuth = context.GetEndpoint()?
                    .Metadata
                    .GetOrderedMetadata<IAuthorizeData>()
                    .Any() == true;

                if (!hasAuthCookies && !endpointRequiresAuth)
                {
                    await _next(context);
                    return;
                }

                if (!context.Request.Cookies.TryGetValue(_settings.Cookies.CsrfKey, out var csrfCookie)
                    || string.IsNullOrWhiteSpace(csrfCookie))
                    throw new ForbiddenException("CSRF token ausente.");

                var csrfHeader = GetCsrfHeader(context.Request.Headers);
                if (string.IsNullOrWhiteSpace(csrfHeader))
                {
                    if (!IsSameOriginRequest(context.Request))
                        throw new ForbiddenException("CSRF token ausente.");

                    await _next(context);
                    return;
                }

                if (!FixedTimeEquals(csrfCookie, csrfHeader))
                    throw new ForbiddenException("CSRF token inválido.");

                await _next(context);
            }

            /// <summary>Operação para obter o valor do header CSRF informado na requisição.</summary>
            /// <param name="headers">Coleção de headers da requisição.</param>
            private static string? GetCsrfHeader(IHeaderDictionary headers)
            {
                foreach (var headerName in CSRF_HEADERS)
                {
                    if (headers.TryGetValue(headerName, out var headerValue)
                        && !string.IsNullOrWhiteSpace(headerValue))
                    {
                        return headerValue.ToString();
                    }
                }

                return null;
            }

            /// <summary>Operação para validar se a requisição é same-origin.</summary>
            /// <param name="request">Requisição HTTP atual.</param>
            private static bool IsSameOriginRequest(HttpRequest request)
            {
                var host = request.Host.Value;
                if (string.IsNullOrWhiteSpace(host))
                    return false;

                var expectedOrigin = $"{request.Scheme}://{host}";

                if (request.Headers.TryGetValue("Origin", out var origin)
                    && !string.IsNullOrWhiteSpace(origin))
                {
                    return string.Equals(origin.ToString(), expectedOrigin, StringComparison.OrdinalIgnoreCase);
                }

                if (request.Headers.TryGetValue("Referer", out var referer)
                    && !string.IsNullOrWhiteSpace(referer)
                    && Uri.TryCreate(referer.ToString(), UriKind.Absolute, out var refererUri))
                {
                    var refererOrigin = $"{refererUri.Scheme}://{refererUri.Authority}";
                    return string.Equals(refererOrigin, expectedOrigin, StringComparison.OrdinalIgnoreCase);
                }

                return false;
            }

            /// <summary>Operação para comparar tokens em tempo constante.</summary>
            /// <param name="left">Token esperado.</param>
            /// <param name="right">Token recebido.</param>
            private static bool FixedTimeEquals(string left, string right)
            {
                var leftBytes = Encoding.UTF8.GetBytes(left);
                var rightBytes = Encoding.UTF8.GetBytes(right);

                if (leftBytes.Length != rightBytes.Length)
                    return false;

                return CryptographicOperations.FixedTimeEquals(leftBytes, rightBytes);
            }
        }
    }
}
