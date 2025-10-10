namespace AuthCore.Domain.Commons.Interfaces.Http
{
    public interface ICookie
    {
        string Session { get; }
        string AccessToken { get; }
        string RefreshToken { get; }
        void SetAuthCookies(string rawSession, string accessToken, string refreshToken);
        void RemoveAuthCookies();
    }
}
