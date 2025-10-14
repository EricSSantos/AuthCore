using AuthCore.Domain.Commons.Interfaces.Security.Hashing;

namespace AuthCore.Infrastructure.Security.Hashing
{
    public sealed class BCryptService : IBCrypt
    {
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool isValid(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
