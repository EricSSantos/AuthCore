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
        private readonly ISessionState _sessionState;
        private readonly IRateLimitStore _rateLimit;
        private readonly SecuritySettings _settings;
        private readonly ILogger<Refresh> _logger;

        public Refresh(
            ISessionService sessionService,
            IJwtTokenProvider jwtTokenProvider,
            ISessionState sessionState,
            IRateLimitStore rateLimit,
            SecuritySettings settings,
            ILogger<Refresh> logger)
        {
            _sessionService = sessionService;
            _jwtTokenProvider = jwtTokenProvider;
            _sessionState = sessionState;
            _rateLimit = rateLimit;
            _settings = settings;
            _logger = logger;
        }

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

            var sessionResult = await _sessionService.CreateSessionAsync(
                user.Id,
                session.DeviceInfo,
                TimeSpan.FromDays(_settings.Session.ExpiresInDays),
                TimeSpan.FromDays(_settings.Session.MaxLifetimeInDays),
                utcNow
            );

            await _sessionService.InvalidateSessionAsync(session.Id, utcNow);

            var newAccessToken = _jwtTokenProvider.Generate(user.Id);

            _sessionState.SetCookies(sessionResult.RawSession, newAccessToken);

            _logger.LogInformation("Refresh realizado. UserId={UserId}.", user.Id);
        }
    }
}
