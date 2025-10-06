using AuthCore.Domain.Interfaces.Security.Cryptography;
using AuthCore.Domain.Interfaces.Security.Tokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace AuthCore.Infrastructure.Security.Tokens
{
    public sealed class RefreshTokenService : IRefreshToken
    {
        private readonly IHmac _hmac;

        public RefreshTokenService(IHmac hmac)
        {
            _hmac = hmac ?? throw new ArgumentNullException(nameof(hmac));
        }

        public (string RawToken, string HashedToken) Generate()
        {
            var rawBytes = RandomNumberGenerator.GetBytes(64);
            var rawToken = Base64UrlEncoder.Encode(rawBytes);
            var hashedToken = _hmac.ComputeBase64(rawBytes);

            return (rawToken, hashedToken);
        }

        public bool Verify(string rawToken, string hashedToken)
        {
            if (string.IsNullOrWhiteSpace(rawToken) || string.IsNullOrWhiteSpace(hashedToken))
                return false;

            return _hmac.Verify(rawToken, hashedToken);
        }
    }
}
