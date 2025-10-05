using AuthCore.Domain.Interfaces.Adapters.Security.Cripto;
using AuthCore.Domain.Interfaces.Adapters.Security.Tokens;
using AuthCore.Domain.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace AuthCore.Infrastructure.Adapters.Security.Tokens
{
    public sealed class AccessTokenAdapter : IAccessTokenAdapter
    {
        private readonly IRsaAdapter _rsa;
        private readonly IHttpContextAccessor _httpContext;
        private readonly SecuritySettings _settings;
        private readonly SigningCredentials _credentials;
        private readonly JwtSecurityTokenHandler _handler = new();

        public AccessTokenAdapter(
            IRsaAdapter rsa,
            IHttpContextAccessor httpContext,
            SecuritySettings settings)
        {
            _rsa = rsa;
            _settings = settings;
            _httpContext = httpContext;

            var rsaPriv = RSA.Create();
            rsaPriv.ImportPkcs8PrivateKey(_rsa.GetPrivateKey(), out _);
            var privateKey = new RsaSecurityKey(rsaPriv);

            _credentials = new SigningCredentials(privateKey, SecurityAlgorithms.RsaSha256);
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
                expires: now.AddMinutes(_settings.Lifetime.ExpirationInMinutes).UtcDateTime,
                signingCredentials: _credentials
            );

            return _handler.WriteToken(token);
        }

        public Guid GetUserId()
        {
            var sub = FindClaimValue(GetPrincipal(), JwtRegisteredClaimNames.Sub);
            if (!Guid.TryParse(sub, out var userId))
                throw new UnauthorizedAccessException("Claim 'sub' ausente ou inválida.");

            return userId;
        }

        public Guid GetJti()
        {
            var jti = FindClaimValue(GetPrincipal(), JwtRegisteredClaimNames.Jti);
            if (!Guid.TryParse(jti, out var jtiId))
                throw new UnauthorizedAccessException("Claim 'jti' ausente ou inválida.");

            return jtiId;
        }

        public long GetIssuedAt()
        {
            var iat = FindClaimValue(GetPrincipal(), JwtRegisteredClaimNames.Iat);
            if (!long.TryParse(iat, out var issuedAt))
                throw new UnauthorizedAccessException("Claim 'iat' ausente ou inválida.");

            return issuedAt;
        }

        #region Private Methods

        private ClaimsPrincipal GetPrincipal()
        {
            var principal = _httpContext.HttpContext?.User;
            if (principal is null || principal.Identity?.IsAuthenticated is not true)
                throw new UnauthorizedAccessException("Usuário não autenticado ou contexto inválido.");

            return principal;
        }

        private static string? FindClaimValue(ClaimsPrincipal principal, string claimType)
        {
            // tenta buscar pelo nome original
            var claim = principal.Claims.FirstOrDefault(c => c.Type == claimType);

            // fallback para equivalentes padrão (por segurança)
            if (claim is null)
            {
                if (claimType == JwtRegisteredClaimNames.Sub)
                    claim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

                else if (claimType == JwtRegisteredClaimNames.Jti)
                    claim = principal.Claims.FirstOrDefault(c => c.Type == "jti");

                else if (claimType == JwtRegisteredClaimNames.Iat)
                    claim = principal.Claims.FirstOrDefault(c => c.Type == "iat");
            }

            return claim?.Value;
        }

        #endregion
    }
}
