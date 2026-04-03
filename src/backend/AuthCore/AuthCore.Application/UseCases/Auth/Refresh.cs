using AuthCore.Application.UseCases.Auth.Interfaces;
using AuthCore.Domain.Aggregates.Sessions.Interfaces;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;
using AuthCore.Domain.Core.Settings;
using Microsoft.Extensions.Logging;

namespace AuthCore.Application.UseCases.Auth
{
    /// <summary>Representa caso de uso de renovação de token.</summary>
    public sealed class Refresh : IRefresh
    {
        private readonly ISessionService _sessionService;
        private readonly IJwtTokenProvider _jwtTokenProvider;
        private readonly ISecureKeyGenerator _secureKeyGenerator;
        private readonly ISessionState _sessionState;
        private readonly IRateLimitStore _rateLimit;
        private readonly SecuritySettings _settings;
        private readonly ILogger<Refresh> _logger;

        /// <summary>Operação para criar instância de caso de uso.</summary>
        /// <param name="sessionService">Serviço de domínio de sessões.</param>
        /// <param name="jwtTokenProvider">Provedor de token JWT.</param>
        /// <param name="secureKeyGenerator">Gerador seguro de chaves.</param>
        /// <param name="sessionState">Gerenciador de estado da sessão atual.</param>
        /// <param name="rateLimit">Armazenamento de limitação de taxa.</param>
        /// <param name="settings">Configurações de segurança.</param>
        /// <param name="logger">Serviço de logging da operação.</param>
        public Refresh(
            ISessionService sessionService,
            IJwtTokenProvider jwtTokenProvider,
            ISecureKeyGenerator secureKeyGenerator,
            ISessionState sessionState,
            IRateLimitStore rateLimit,
            SecuritySettings settings,
            ILogger<Refresh> logger)
        {
            _sessionService = sessionService;
            _jwtTokenProvider = jwtTokenProvider;
            _secureKeyGenerator = secureKeyGenerator;
            _sessionState = sessionState;
            _rateLimit = rateLimit;
            _settings = settings;
            _logger = logger;
        }

        /// <summary>Operação para renovar o token do usuário.</summary>
        public async Task OnExecuteAsync()
        {
            var result = await _sessionState.GetCurrentUserFromSession();
            var user = result.User;
            var session = result.Session;
            var utcNow = DateTime.UtcNow;

            if (_settings.Abuse.RefreshPerMinute > 0)
            {
                await _rateLimit.EnsureFixedWindowAsync(
                    key: $"auth:refresh:session:{session.Id}",
                    limit: _settings.Abuse.RefreshPerMinute,
                    window: TimeSpan.FromMinutes(1),
                    errorMessage: "Muitas requisições de refresh. Tente novamente mais tarde.");
            }

            var rawSession = _secureKeyGenerator.Generate();
            var sessionId = _secureKeyGenerator.Hash(rawSession);

            await _sessionService.InvalidateSessionAsync(session.Id.Value, utcNow);

            var sessionResult = await _sessionService.CreateSessionAsync(
                user.Id,
                sessionId,
                rawSession,
                session.DeviceInfo,
                TimeSpan.FromDays(_settings.Session.ExpiresInDays),
                TimeSpan.FromDays(_settings.Session.MaxLifetimeInDays),
                utcNow
            );

            var newAccessToken = _jwtTokenProvider.Generate(user.Id);

            _sessionState.SetCookies(sessionResult.RawSession, newAccessToken);
            _logger.LogInformation("Refresh realizado. UserId={UserId}.", user.Id);
        }
    }
}
