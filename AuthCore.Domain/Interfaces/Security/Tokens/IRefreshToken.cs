namespace AuthCore.Domain.Interfaces.Security.Tokens
{
    public interface IRefreshToken
    {
        (string RawToken, string HashedToken) Generate();
        bool Verify(string rawToken, string hashedToken);
    }
}
