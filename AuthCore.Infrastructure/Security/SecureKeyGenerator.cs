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

        public SecureKeyGenerator(IOptions<SecuritySettings> settings)
        {
            var symmetricKey = settings.Value.Keys.Symmetric.PrivateKey;

            if (string.IsNullOrWhiteSpace(symmetricKey))
                throw new ArgumentException("A chave simétrica HMAC não foi configurada.");

            _key = Encoding.UTF8.GetBytes(symmetricKey);
        }

        public string Generate(int size = 32)
        {
            var bytes = RandomNumberGenerator.GetBytes(size);
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }

        public string Hash(string raw)
        {
            var bytes = Convert.FromHexString(raw);
            return ComputeHash(bytes);
        }

        public bool Verify(string raw, string hash)
        {
            var bytes = Convert.FromHexString(raw);
            var computed = ComputeHash(bytes);

            var expected = Base64UrlEncoder.DecodeBytes(hash);
            var actual = Base64UrlEncoder.DecodeBytes(computed);

            return CryptographicOperations.FixedTimeEquals(actual, expected);
        }

        #region Helpers

        private string ComputeHash(byte[] data)
        {
            using var hmac = new HMACSHA256(_key);
            var hashBytes = hmac.ComputeHash(data);
            return Base64UrlEncoder.Encode(hashBytes);
        }

        #endregion
    }
}
