namespace AuthCore.Domain.Interfaces.Adapters.Security.Tokens
{
    public interface IRefreshTokenAdapter
    {
        (string RawToken, string HashedToken) GenerateRefreshToken();
        bool ValidateRefreshToken(string rawToken, string hashedToken);
    }
}
