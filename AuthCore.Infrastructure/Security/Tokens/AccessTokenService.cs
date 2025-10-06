using AuthCore.Domain.Interfaces.Security.Cryptography;
using AuthCore.Domain.Interfaces.Security.Tokens;
using AuthCore.Domain.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace AuthCore.Infrastructure.Security.Tokens
{
    public sealed class AccessTokenService : IAccessToken
    {
        private readonly IEcdsaProvider _ecdsa;
        private readonly IHttpContextAccessor _httpContext;
        private readonly SecuritySettings _settings;
        private readonly SigningCredentials _credentials;
        private readonly JwtSecurityTokenHandler _handler;

        public AccessTokenService(IEcdsaProvider ecdsa, IHttpContextAccessor httpContext, IOptions<SecuritySettings> settings)
        {
            _ecdsa = ecdsa;
            _httpContext = httpContext;
            _settings = settings.Value;
            _handler = new JwtSecurityTokenHandler();

            var ecdsaPriv = ECDsa.Create();
            ecdsaPriv.ImportPkcs8PrivateKey(_ecdsa.PrivateKey, out _);
            var ecKey = new ECDsaSecurityKey(ecdsaPriv);

            _credentials = new SigningCredentials(ecKey, SecurityAlgorithms.EcdsaSha256);
        }

        public Guid Sub
        {
            get
            {
                var sub = GetClaimValue(JwtRegisteredClaimNames.Sub, ClaimTypes.NameIdentifier);
                return ParseGuidClaim(sub, "sub");
            }
        }

        public Guid Jti
        {
            get
            {
                var jti = GetClaimValue(JwtRegisteredClaimNames.Jti, "jti");
                return ParseGuidClaim(jti, "jti");
            }
        }

        public long IssuedAt
        {
            get
            {
                var iat = GetClaimValue(JwtRegisteredClaimNames.Iat, "iat");
                return ParseLongClaim(iat, "iat");
            }
        }

        public string Generate(Guid userId)
        {
            var now = DateTimeOffset.UtcNow;

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Iat, now.ToUnixTimeSeconds().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _settings.Jwt.Issuer,
                audience: _settings.Jwt.Audience,
                claims: claims,
                expires: now.AddMinutes(15).UtcDateTime,
                signingCredentials: _credentials
            );

            return _handler.WriteToken(token);
        }

        #region Private Methods

        private ClaimsPrincipal GetPrincipal()
        {
            var principal = _httpContext.HttpContext?.User;

            if (principal is null || principal.Identity?.IsAuthenticated is not true)
                throw new UnauthorizedAccessException("Usuário não autenticado ou contexto inválido.");

            return principal;
        }

        private string? GetClaimValue(string standardClaim, string fallbackClaim)
        {
            var principal = GetPrincipal();

            var claim = principal.Claims.FirstOrDefault(c => c.Type == standardClaim);

            if (claim == null)
                claim = principal.Claims.FirstOrDefault(c => c.Type == fallbackClaim);

            return claim?.Value;
        }

        private static Guid ParseGuidClaim(string? value, string claimName)
        {
            if (!Guid.TryParse(value, out var result))
                throw new UnauthorizedAccessException($"Claim '{claimName}' ausente ou inválida.");

            return result;
        }

        private static long ParseLongClaim(string? value, string claimName)
        {
            if (!long.TryParse(value, out var result))
                throw new UnauthorizedAccessException($"Claim '{claimName}' ausente ou inválida.");

            return result;
        }

        #endregion
    }
}
