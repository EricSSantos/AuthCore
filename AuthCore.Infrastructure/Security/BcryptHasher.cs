using AuthCore.Domain.Interfaces.Security;

namespace AuthCore.Infrastructure.Security
{
    public sealed class BcryptHasher : IBcryptHasher
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
