using AuthCore.Domain.Interfaces.Adapters.Security.Tokens;
using AuthCore.Domain.Settings;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace AuthCore.Infrastructure.Adapters.Security.Tokens
{
    public sealed class RefreshTokenAdapter : IRefreshTokenAdapter
    {
        private readonly byte[] _key;

        public RefreshTokenAdapter(SecuritySettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.Keys.Symmetric.PrivateKey))
                throw new InvalidOperationException("A chave simétrica para RefreshToken não foi configurada.");

            _key = Encoding.UTF8.GetBytes(settings.Keys.Symmetric.PrivateKey);
        }

        public (string RawToken, string HashedToken) GenerateRefreshToken()
        {
            var rawBytes = RandomNumberGenerator.GetBytes(64);
            var rawToken = Base64UrlEncoder.Encode(rawBytes);

            using var hmac = new HMACSHA512(_key);
            var hashBytes = hmac.ComputeHash(rawBytes);

            var hashedToken = Base64UrlEncoder.Encode(hashBytes);

            return (rawToken, hashedToken);
        }

        public bool ValidateRefreshToken(string rawToken, string hashedToken)
        {
            var rawBytes = Base64UrlEncoder.DecodeBytes(rawToken);

            using var hmac = new HMACSHA512(_key);
            var computedHash = hmac.ComputeHash(rawBytes);

            return CryptographicOperations.FixedTimeEquals(computedHash, Base64UrlEncoder.DecodeBytes(hashedToken));
        }
    }
}
