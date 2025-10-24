using AuthCore.Domain.Commons.Interfaces.Http;
using Microsoft.AspNetCore.Http;

namespace AuthCore.Infrastructure.Http
{
    public sealed class CookieService : ICookie
    {
        #region Constants

        public const string SESSION_ID = "session";
        public const string ACCESS_TOKEN = "access_token";
        public const string REFRESH_TOKEN = "refresh_token";

        #endregion

        private readonly IHttpContextAccessor _http;

        public CookieService(IHttpContextAccessor http)
        {
            _http = http;
        }

        public string Session
        {
            get { return Get(SESSION_ID); }
        }

        public string AccessToken
        {
            get { return Get(ACCESS_TOKEN); }
        }

        public string RefreshToken
        {
            get { return Get(REFRESH_TOKEN); }
        }

        public void SetAuthCookies(string rawSession, string accessToken, string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(rawSession) ||
                string.IsNullOrWhiteSpace(accessToken) ||
                string.IsNullOrWhiteSpace(refreshToken))
                throw new ArgumentException("Os cookies de autenticação não podem ser nulos ou vazios.");

            var ttlDays = TimeSpan.FromDays(7);
            var ttlMinutes = TimeSpan.FromMinutes(15);

            Set(SESSION_ID, rawSession, ttlDays);
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
                    ? DateTime.UtcNow.Add(expires.Value)
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
