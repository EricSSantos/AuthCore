namespace AuthCore.Domain.Interfaces.Security.Sessions
{
    public interface ISessionIdentity
    {
        (string RawSession, string HashedSession) Generate();
        bool Verify(string rawSession, string hashedSession);
    }
}
