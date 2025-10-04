using AuthCore.Domain.Interfaces.Adapters.Security.Cripto;

namespace AuthCore.Infrastructure.Adapters.Security.Cripto
{
    public sealed class BCryptAdapter : IBCryptAdapter
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
