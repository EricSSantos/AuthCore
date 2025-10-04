namespace AuthCore.Domain.Interfaces.Adapters.Security.Tokens
{
    public interface IAccessTokenAdapter
    {
        string GenerateAccessToken(Guid userId);
    }
}
