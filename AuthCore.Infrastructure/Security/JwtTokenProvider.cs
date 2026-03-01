using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;
using AuthCore.Domain.Core.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
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

        /// <summary>Operação para criar instância do provedor de criação e leitura de JWT.</summary>
        /// <param name="ecdsa">Provedor de chaves ECDSA.</param>
        /// <param name="httpContext">Acessor de contexto HTTP.</param>
        /// <param name="settings">Configurações de segurança.</param>
        /// <param name="logger">Serviço de logging.</param>
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
            ECDsa ecdsaPrivateKey = ECDsa.Create();
            ecdsaPrivateKey.ImportPkcs8PrivateKey(_ecdsa.PrivateKey, out _);
            ECDsaSecurityKey ecdsaSecurityKey = new ECDsaSecurityKey(ecdsaPrivateKey);
            _credentials = new SigningCredentials(ecdsaSecurityKey, SecurityAlgorithms.EcdsaSha256);
        }

        /// <summary>Operação para obter identificador do usuário autenticado a partir do token JWT.</summary>
        public Guid Sub
        {
            get
            {
                string? claimValue = GetClaimValue(JwtRegisteredClaimNames.Sub, ClaimTypes.NameIdentifier);

                if (string.IsNullOrWhiteSpace(claimValue) || !Guid.TryParse(claimValue, out Guid userId))
                {
                    _logger.LogWarning("Claim 'sub' ausente ou inválido.");
                    throw new UnauthorizedException();
                }

                return userId;
            }
        }

        /// <summary>Operação para gerar token de acesso.</summary>
        /// <param name="userId">Identificador do usuário.</param>
        public string Generate(Guid userId)
        {
            List<Claim> claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            JwtSecurityToken token = new JwtSecurityToken(
                issuer: _settings.Jwt.Issuer,
                audience: _settings.Jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_settings.Jwt.ExpiresInMinutes),
                signingCredentials: _credentials);

            _logger.LogInformation("JWT gerado para usuário {UserId}.", userId);

            return _handler.WriteToken(token);
        }

        /// <summary>Operação para obter usuário autenticado do contexto HTTP atual.</summary>
        private ClaimsPrincipal GetPrincipal()
        {
            ClaimsPrincipal? principal = _httpContext.HttpContext?.User;

            if (principal is null || principal.Identity?.IsAuthenticated is not true)
            {
                _logger.LogWarning("Usuário não autenticado ou contexto inválido.");
                throw new UnauthorizedException();
            }

            return principal;
        }

        /// <summary>Operação para obter valor de claim do token JWT.</summary>
        /// <param name="standardClaim">Nome padrão da claim.</param>
        /// <param name="fallbackClaim">Nome alternativo da claim.</param>
        private string? GetClaimValue(string standardClaim, string fallbackClaim)
        {
            ClaimsPrincipal principal = GetPrincipal();

            Claim? claim = principal.Claims.FirstOrDefault(c => c.Type == standardClaim);
            if (claim is null)
                claim = principal.Claims.FirstOrDefault(c => c.Type == fallbackClaim);

            return claim?.Value;
        }
    }
}