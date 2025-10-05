using AuthCore.Domain.Interfaces.Adapters.Http;
using AuthCore.Domain.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace AuthCore.Infrastructure.Adapters.Http
{
    public sealed class CookieAdapter : ICookieAdapter
    {
        #region Constants

        public const string SESSION_ID = "session";
        public const string ACCESS_TOKEN = "access_token";
        public const string REFRESH_TOKEN = "refresh_token";

        #endregion

        private readonly IHttpContextAccessor _http;
        private readonly LifetimeSettings _lifetime;

        public CookieAdapter(IHttpContextAccessor http, IOptions<SecuritySettings> settings)
        {
            _http = http;
            _lifetime = settings.Value.Lifetime;
        }

        public Guid GetSessionId()
        {
            var value = Get(SESSION_ID);
            if (!Guid.TryParse(value, out var sid))
                throw new UnauthorizedAccessException("Identificador de sessão inválido.");

            return sid;
        }

        public string GetAccessToken()
        {
            return Get(ACCESS_TOKEN);
        }

        public string GetRefreshToken()
        {
            return Get(REFRESH_TOKEN);
        }

        public void SetAuthCookies(Guid sessionId, string accessToken, string refreshToken)
        {
            var ttlDays = TimeSpan.FromDays(_lifetime.ExpirationInDays);
            var ttlMinutes = TimeSpan.FromMinutes(_lifetime.ExpirationInMinutes);

            Set(SESSION_ID, sessionId.ToString(), ttlDays);
            Set(ACCESS_TOKEN, accessToken, ttlMinutes);
            Set(REFRESH_TOKEN, refreshToken, ttlDays);
        }

        public void RemoveAuthCookies()
        {
            Remove(SESSION_ID);
            Remove(ACCESS_TOKEN);
            Remove(REFRESH_TOKEN);
        }

        #region Private Methods

        private HttpContext GetContext()
        {
            return _http.HttpContext
                ?? throw new InvalidOperationException("Contexto HTTP indisponível.");
        }

        private string Get(string key)
        {
            var context = GetContext();

            if (!context.Request.Cookies.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
                throw new UnauthorizedAccessException($"'{key}' ausente ou inválido.");

            return value.Trim();
        }

        private void Set(string key, string value, TimeSpan? expires = null, bool httpOnly = true, bool secure = true)
        {
            var context = GetContext();

            var options = new CookieOptions
            {
                HttpOnly = httpOnly,
                Secure = secure,
                SameSite = SameSiteMode.Strict,
                Expires = expires.HasValue
                    ? DateTimeOffset.UtcNow.Add(expires.Value)
                    : null
            };

            context.Response.Cookies.Append(key, value, options);
        }

        private void Remove(string key)
        {
            var context = GetContext();
            context.Response.Cookies.Delete(key);
        }

        #endregion
    }
}
