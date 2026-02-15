using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;

namespace AuthCore.Infrastructure.Security
{
    /// <summary>Representa serviço de hash de senha.</summary>
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
