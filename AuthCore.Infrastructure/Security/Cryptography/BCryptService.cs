using AuthCore.Domain.Commons.Interfaces.Security.Cryptography;

namespace AuthCore.Infrastructure.Security.Cryptography
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
