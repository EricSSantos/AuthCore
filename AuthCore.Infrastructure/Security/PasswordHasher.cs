using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;

namespace AuthCore.Infrastructure.Security
{
    public sealed class PasswordHasher : IPasswordHasher
    {
        public string Hash(string plainText)
        {
            return BCrypt.Net.BCrypt.HashPassword(plainText);
        }

        public bool IsValid(string plainText, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(plainText, passwordHash);
        }
    }
}
