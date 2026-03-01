using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;

namespace AuthCore.Infrastructure.Security
{
    /// <summary>Representa serviço de hash de senha.</summary>
    public sealed class PasswordHasher : IPasswordHasher
    {
        /// <summary>Operação para gerar hash da senha informada.</summary>
        /// <param name="plainText">Senha em texto puro.</param>
        public string Hash(string plainText)
        {
            return BCrypt.Net.BCrypt.HashPassword(plainText);
        }

        /// <summary>Operação para validar senha informada com o hash armazenado.</summary>
        /// <param name="plainText">Senha em texto puro.</param>
        /// <param name="passwordHash">Hash da senha armazenada.</param>
        public bool IsValid(string plainText, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(plainText, passwordHash);
        }
    }
}