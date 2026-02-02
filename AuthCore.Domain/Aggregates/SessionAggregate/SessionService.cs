using AuthCore.Domain.Aggregates.SessionAggregate.Interfaces;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;

namespace AuthCore.Domain.Aggregates.SessionAggregate
{
    /// <summary>
    /// Coordena regras de domínio relacionadas ao ciclo de vida de sessões.
    /// </summary>
    public sealed class SessionService : ISessionService
    {
        private const int MAX_SESSIONS_PER_USER = 4;

        private readonly ISessionRepository _sessionRepository;
        private readonly ISecureKeyGenerator _secureKeyGenerator;

        public SessionService(
            ISessionRepository sessionRepository,
            ISecureKeyGenerator secureKeyGenerator)
        {
            _sessionRepository = sessionRepository;
            _secureKeyGenerator = secureKeyGenerator;
        }

        public async Task<SessionCreationResult> CreateSessionAsync(Guid userId, DeviceInfo deviceInfo, TimeSpan ttl, TimeSpan maxLifetime)
        {
            var sessions = (await _sessionRepository.GetAllByUserIdAsync(userId))
                .OrderBy(s => s.CreatedAt)
                .ToList();

            if (sessions.Count >= MAX_SESSIONS_PER_USER)
            {
                var sessionsToRevoke = sessions
                    .Take(sessions.Count - MAX_SESSIONS_PER_USER + 1);

                foreach (var oldSession in sessionsToRevoke)
                {
                    oldSession.Revoke(DateTime.UtcNow);
                    await _sessionRepository.SetAsync(oldSession);
                }
            }

            var rawSession = _secureKeyGenerator.Generate();
            var sessionHash = _secureKeyGenerator.Hash(rawSession);

            var session = Session.Create(
                id: sessionHash,
                userId: userId,
                deviceInfo: deviceInfo,
                ttl: ttl,
                maxLifetime: maxLifetime
            );

            await _sessionRepository.SetAsync(session);

            return new SessionCreationResult(session, rawSession);
        }

        public async Task InvalidateSessionAsync(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
                return;

            var session = await _sessionRepository.GetAsync(sessionId);
            if (session is null)
                return;

            session.Revoke(DateTime.UtcNow);

            await _sessionRepository.SetAsync(session);
        }
    }
}
