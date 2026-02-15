using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;
using AuthCore.Domain.Core.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace AuthCore.Infrastructure.Security
{
    /// <summary>Representa serviço de geração de chaves seguras.</summary>
    public sealed class SecureKeyGenerator : ISecureKeyGenerator
    {
        private readonly byte[] _key;

        /// <summary>Operação para criar instância do gerador de chaves.</summary>
        /// <param name="settings">Configurações de segurança.</param>
        public SecureKeyGenerator(IOptions<SecuritySettings> settings)
        {
            var symmetricKey = settings.Value.Keys.Symmetric.PrivateKey;

            if (string.IsNullOrWhiteSpace(symmetricKey))
                throw new ArgumentException("A chave simétrica HMAC não foi configurada.");

            _key = Encoding.UTF8.GetBytes(symmetricKey);
        }

        /// <summary>Operação para gerar chave aleatória em hex.</summary>
        /// <param name="size">Tamanho em bytes.</param>
        public string Generate(int size = 32)
        {
            var bytes = RandomNumberGenerator.GetBytes(size);
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }

        /// <summary>Operação para gerar hash HMAC da chave.</summary>
        /// <param name="raw">Chave em hex.</param>
        public string Hash(string raw)
        {
            var bytes = Convert.FromHexString(raw);
            return ComputeHash(bytes);
        }

        /// <summary>Operação para validar hash HMAC da chave.</summary>
        /// <param name="raw">Chave em hex.</param>
        /// <param name="hash">Hash esperado.</param>
        public bool Verify(string raw, string hash)
        {
            var bytes = Convert.FromHexString(raw);
            var computed = ComputeHash(bytes);

            var expected = Base64UrlEncoder.DecodeBytes(hash);
            var actual = Base64UrlEncoder.DecodeBytes(computed);

            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }

        #region Helpers

        /// <summary>Operação para computar hash HMAC.</summary>
        /// <param name="data">Bytes de entrada.</param>
        private string ComputeHash(byte[] data)
        {
            using var hmac = new HMACSHA256(_key);
            var hashBytes = hmac.ComputeHash(data);
            return Base64UrlEncoder.Encode(hashBytes);
        }

        #endregion
    }
}
