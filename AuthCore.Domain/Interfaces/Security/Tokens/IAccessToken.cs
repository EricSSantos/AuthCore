namespace AuthCore.Domain.Interfaces.Security.Tokens
{
    public interface IAccessToken
    {
        Guid Sub { get; }
        Guid Jti { get; }
        long IssuedAt { get; }
        string Generate(Guid userId);
    }
}
