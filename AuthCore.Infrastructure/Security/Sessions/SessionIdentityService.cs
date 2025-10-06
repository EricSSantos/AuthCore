using AuthCore.Domain.Interfaces.Security.Cryptography;
using AuthCore.Domain.Interfaces.Security.Sessions;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace AuthCore.Infrastructure.Security.Sessions
{
    public sealed class SessionIdentityService : ISessionIdentity
    {
        private readonly IHmac _hmac;

        public SessionIdentityService(IHmac hmac)
        {
            _hmac = hmac;
        }

        public (string RawSession, string HashedSession) Generate()
        {
            var rawBytes = RandomNumberGenerator.GetBytes(32);
            var rawSession = Base64UrlEncoder.Encode(rawBytes);
            var hashedSession = _hmac.ComputeBase64(rawBytes);

            return (rawSession, hashedSession);
        }

        public bool Verify(string rawSession, string hashedSession)
        {
            if (string.IsNullOrWhiteSpace(rawSession) || string.IsNullOrWhiteSpace(hashedSession))
                return false;

            return _hmac.Verify(rawSession, hashedSession);
        }
    }
}
