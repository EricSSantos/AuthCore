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

        /// <summary>Operação para criar instância do gerador de chaves seguras.</summary>
        /// <param name="settings">Configurações de segurança.</param>
        public SecureKeyGenerator(IOptions<SecuritySettings> settings)
        {
            string symmetricKey = settings.Value.Keys.Symmetric.PrivateKey;

            if (string.IsNullOrWhiteSpace(symmetricKey))
                throw new ArgumentException("A chave simétrica HMAC não foi configurada.");

            _key = Encoding.UTF8.GetBytes(symmetricKey);
        }

        /// <summary>Operação para gerar chave aleatória em formato hexadecimal.</summary>
        /// <param name="size">Tamanho da chave em bytes.</param>
        public string Generate(int size = 32)
        {
            byte[] bytes = RandomNumberGenerator.GetBytes(size);
            return Convert.ToHexString(bytes).ToLowerInvariant();
        }

        /// <summary>Operação para gerar hash HMAC da chave informada.</summary>
        /// <param name="raw">Chave em formato hexadecimal.</param>
        public string Hash(string raw)
        {
            byte[] bytes = Convert.FromHexString(raw);

            return ComputeHash(bytes);
        }

        /// <summary>Operação para validar hash HMAC da chave informada.</summary>
        /// <param name="raw">Chave em formato hexadecimal.</param>
        /// <param name="hash">Hash esperado.</param>
        public bool Verify(string raw, string hash)
        {
            byte[] bytes = Convert.FromHexString(raw);
            string computedHash = ComputeHash(bytes);

            byte[] expectedHashBytes = Base64UrlEncoder.DecodeBytes(hash);
            byte[] actualHashBytes = Base64UrlEncoder.DecodeBytes(computedHash);

            return CryptographicOperations.FixedTimeEquals(actualHashBytes, expectedHashBytes);
        }

        /// <summary>Operação para computar hash HMAC.</summary>
        /// <param name="data">Bytes de entrada.</param>
        private string ComputeHash(byte[] data)
        {
            using HMACSHA256 hmac = new HMACSHA256(_key);

            byte[] hashBytes = hmac.ComputeHash(data);

            return Base64UrlEncoder.Encode(hashBytes);
        }
    }
}