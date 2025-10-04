namespace AuthCore.Domain.Interfaces.Adapters.Http
{
    public interface ICookieAdapter
    {
        string? GetSessionId();
        string? GetAccessToken();
        string? GetRefreshToken();
        void SetAuthCookies(Guid sessionId, string accessToken, string refreshToken);
        void RemoveAuthCookies();
    }
}
