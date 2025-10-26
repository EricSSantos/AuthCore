using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Http;
using Microsoft.AspNetCore.Http;

namespace AuthCore.Infrastructure.Http
{
    public sealed class CookieService : ICookie
    {
        private readonly IHttpContextAccessor _http;

        public CookieService(IHttpContextAccessor http)
        {
            _http = http;
        }

        public string Get(string key)
        {
            var context = EnsureContext();
            if (!context.Request.Cookies.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
                throw new UnauthorizedException($"Cookie '{key}' ausente ou inválido.");

            return value.Trim();
        }

        public void Set(string key, string value, TimeSpan ttl)
        {
            var context = EnsureContext();

            var options = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.Add(ttl),
                IsEssential = true,
                Path = "/"
            };

            context.Response.Cookies.Append(key, value, options);
        }

        public void Remove(string key)
        {
            var context = EnsureContext();

            var options = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(-1),
                Path = "/"
            };

            context.Response.Cookies.Delete(key, options);
        }

        #region Helpers

        private HttpContext EnsureContext()
        {
            return _http.HttpContext
                ?? throw new UnauthorizedException("Contexto HTTP indisponível.");
        }

        #endregion
    }
}
