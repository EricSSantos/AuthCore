using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Web;
using Microsoft.AspNetCore.Http;

namespace AuthCore.Infrastructure.Web
{
    /// <summary>Representa serviço de cookies HTTP.</summary>
    public sealed class CookieService : ICookie
    {
        private readonly IHttpContextAccessor _http;

        /// <summary>Operação para criar instância do serviço de cookies.</summary>
        /// <param name="http">Acessor de contexto HTTP.</param>
        public CookieService(IHttpContextAccessor http)
        {
            _http = http;
        }

        /// <summary>Operação para obter valor de cookie.</summary>
        /// <param name="key">Nome do cookie.</param>
        public string Get(string key)
        {
            var context = EnsureContext();
            if (!context.Request.Cookies.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
                throw new UnauthorizedException();

            return value.Trim();
        }

        /// <summary>Operação para definir cookie com TTL.</summary>
        /// <param name="key">Nome do cookie.</param>
        /// <param name="value">Valor do cookie.</param>
        /// <param name="ttl">Tempo de vida do cookie.</param>
        public void Set(string key, string value, TimeSpan ttl)
        {
            Set(key, value, ttl, httpOnly: true);
        }

        /// <summary>Operação para definir cookie com controle de HttpOnly.</summary>
        /// <param name="key">Nome do cookie.</param>
        /// <param name="value">Valor do cookie.</param>
        /// <param name="ttl">Tempo de vida do cookie.</param>
        /// <param name="httpOnly">Define se o cookie é HttpOnly.</param>
        public void Set(string key, string value, TimeSpan ttl, bool httpOnly)
        {
            var context = EnsureContext();

            var options = new CookieOptions
            {
                HttpOnly = httpOnly,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.Add(ttl),
                IsEssential = true,
                Path = "/"
            };

            context.Response.Cookies.Append(key, value, options);
        }

        /// <summary>Operação para remover cookie.</summary>
        /// <param name="key">Nome do cookie.</param>
        public void Remove(string key)
        {
            var context = EnsureContext();
            var options = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.UtcNow.AddDays(-1),
                Path = "/"
            };
            context.Response.Cookies.Delete(key, options);
        }

        /// <summary>Operação para obter o contexto HTTP atual.</summary>
        private HttpContext EnsureContext()
        {
            return _http.HttpContext ?? throw new UnauthorizedException();
        }
    }
}
