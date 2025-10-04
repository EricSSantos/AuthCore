using AuthCore.Domain.Interfaces.Adapters.Security.Cripto;
using AuthCore.Domain.Interfaces.Adapters.Security.Tokens;
using AuthCore.Domain.Settings;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace AuthCore.Infrastructure.Adapters.Security.Tokens
{
    public sealed class AccessTokenAdapter : IAccessTokenAdapter
    {
        private readonly IRsaAdapter _rsa;
        private readonly SecuritySettings _settings;
        private readonly SigningCredentials _credentials;

        public AccessTokenAdapter(IRsaAdapter rsa, SecuritySettings settings)
        {
            _rsa = rsa;
            _settings = settings;

            var rsaPriv = RSA.Create();
            rsaPriv.ImportPkcs8PrivateKey(_rsa.GetPrivateKey(), out _);

            var privateKey = new RsaSecurityKey(rsaPriv);
            _credentials = new SigningCredentials(privateKey, SecurityAlgorithms.RsaSha256);
        }

        public string GenerateAccessToken(Guid userId)
        {
            var dateTime = DateTimeOffset.UtcNow;

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _settings.Jwt.Issuer,
                audience: _settings.Jwt.Audience,
                claims: claims,
                expires: dateTime.AddMinutes(_settings.Lifetime.ExpirationInMinutes).UtcDateTime,
                signingCredentials: _credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
