using AuthCore.Domain.Interfaces.Security.Cryptography;
using AuthCore.Domain.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace AuthCore.Infrastructure.Security.Cryptography
{
    public sealed class HmacService : IHmac
    {
        private readonly byte[] _key;

        public HmacService(IOptions<SecuritySettings> settings)
        {
            var symmetricKey = settings.Value.Keys.Symmetric.PrivateKey;

            if (string.IsNullOrWhiteSpace(symmetricKey))
                throw new ArgumentException("A chave simétrica HMAC não foi configurada.");

            _key = Encoding.UTF8.GetBytes(symmetricKey);
        }

        public string ComputeBase64(byte[] data)
        {
            using var hmac = new HMACSHA256(_key);
            var hash = hmac.ComputeHash(data);
            return Base64UrlEncoder.Encode(hash);
        }

        public bool Verify(string raw, string expected)
        {
            var rawBytes = Base64UrlEncoder.DecodeBytes(raw);
            var computed = ComputeBase64(rawBytes);

            return CryptographicOperations.FixedTimeEquals(
                Base64UrlEncoder.DecodeBytes(computed),
                Base64UrlEncoder.DecodeBytes(expected)
            );
        }
    }
}
