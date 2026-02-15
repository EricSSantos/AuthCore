using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;
using AuthCore.Domain.Core.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace AuthCore.Infrastructure.Security
{
    /// <summary>Representa provedor de criação e leitura de JWT.</summary>
    public sealed class JwtTokenProvider : IJwtTokenProvider
    {
        private readonly IEcdsaProvider _ecdsa;
        private readonly IHttpContextAccessor _httpContext;
        private readonly SecuritySettings _settings;
        private readonly SigningCredentials _credentials;
        private readonly JwtSecurityTokenHandler _handler;
        private readonly ILogger<JwtTokenProvider> _logger;

        public JwtTokenProvider(
            IEcdsaProvider ecdsa,
            IHttpContextAccessor httpContext,
            IOptions<SecuritySettings> settings,
            ILogger<JwtTokenProvider> logger)
        {
            _ecdsa = ecdsa;
            _httpContext = httpContext;
            _settings = settings.Value;
            _handler = new JwtSecurityTokenHandler();
            _logger = logger;

            var ecdsaPriv = ECDsa.Create();
            ecdsaPriv.ImportPkcs8PrivateKey(_ecdsa.PrivateKey, out _);
            var ecKey = new ECDsaSecurityKey(ecdsaPriv);

            _credentials = new SigningCredentials(ecKey, SecurityAlgorithms.EcdsaSha256);
        }

        public Guid Sub
        {
            get
            {
                var claimValue = GetClaimValue(JwtRegisteredClaimNames.Sub, ClaimTypes.NameIdentifier);

                if (string.IsNullOrWhiteSpace(claimValue) || !Guid.TryParse(claimValue, out var userId))
                {
                    _logger.LogWarning("Claim 'sub' ausente ou inválido.");
                    throw new UnauthorizedException("Claim 'sub' ausente ou inválido.");
                }

                return userId;
            }
        }

        public string Generate(Guid userId)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _settings.Jwt.Issuer,
                audience: _settings.Jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_settings.Jwt.ExpiresInMinutes),
                signingCredentials: _credentials
            );

            _logger.LogInformation("JWT gerado para usuário {UserId}.", userId);
            return _handler.WriteToken(token);
        }

        #region Helpers

        private ClaimsPrincipal GetPrincipals()
        {
            var principal = _httpContext.HttpContext?.User;
            if (principal is null || principal.Identity?.IsAuthenticated is not true)
            {
                _logger.LogWarning("Usuário não autenticado ou contexto inválido.");
                throw new UnauthorizedException("Usuário não autenticado ou contexto inválido.");
            }

            return principal;
        }

        private string? GetClaimValue(string standardClaim, string fallbackClaim)
        {
            var principal = GetPrincipals();

            var claim = principal.Claims.FirstOrDefault(c => c.Type == standardClaim);
            if (claim == null)
                claim = principal.Claims.FirstOrDefault(c => c.Type == fallbackClaim);

            return claim?.Value;
        }

        #endregion
    }
}
