namespace AuthCore.Domain.Interfaces.Adapters.Security.Tokens
{
    public interface IAccessTokenAdapter
    {
        string Generate(Guid userId);
        Guid GetUserId();
        Guid GetJti();
        long GetIssuedAt();
    }
}
