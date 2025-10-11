using AuthCore.Domain.Commons.Interfaces.Security;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace AuthCore.Infrastructure.Security.Cryptography
{
    public sealed class EntropyService : IEntropy
    {
        private readonly IHmac _hmac;

        public EntropyService(IHmac hmac)
        {
            _hmac = hmac;
        }

        public (string Raw, string Hash) GeneratePair(int size = 32)
        {
            var rawBytes = RandomNumberGenerator.GetBytes(size);
            var raw = Base64UrlEncoder.Encode(rawBytes);
            var hash = _hmac.ComputeBase64(rawBytes);

            return (raw, hash);
        }

        public string Hash(string raw)
        {
            var bytes = Base64UrlEncoder.DecodeBytes(raw);
            return _hmac.ComputeBase64(bytes);
        }

        public bool Verify(string raw, string hash)
        {
            return _hmac.Verify(raw, hash);
        }
    }
}
