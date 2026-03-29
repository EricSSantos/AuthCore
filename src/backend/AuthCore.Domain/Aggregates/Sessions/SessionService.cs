using AuthCore.Domain.Aggregates.Sessions.Interfaces;
using AuthCore.Domain.Aggregates.Sessions.Results;

namespace AuthCore.Domain.Aggregates.Sessions
{
    /// <summary>Representa serviço de domínio de sessões.</summary>
    public sealed class SessionService : ISessionService
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly ISessionPolicy _sessionPolicy;

        #region Constructors

        /// <summary>Operação para criar instância de serviço de sessões.</summary>
        /// <param name="sessionRepository">Repositório de sessões.</param>
        /// <param name="sessionPolicy">Política de sessões.</param>
        public SessionService(
            ISessionRepository sessionRepository,
            ISessionPolicy sessionPolicy)
        {
            _sessionRepository = sessionRepository;
            _sessionPolicy = sessionPolicy;
        }

        #endregion

        /// <summary>Operação para criar sessão.</summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="sessionId">Identificador da sessão (hash).</param>
        /// <param name="rawSession">Segredo bruto da sessão.</param>
        /// <param name="deviceInfo">Dados do dispositivo.</param>
        /// <param name="ttl">Tempo de expiração da sessão.</param>
        /// <param name="maxLifetime">Tempo máximo de vida útil.</param>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        public async Task<SessionCreationResult> CreateSessionAsync(
            Guid userId,
            string sessionId,
            string rawSession,
            DeviceInfo deviceInfo,
            TimeSpan ttl,
            TimeSpan maxLifetime,
            DateTime utcNow)
        {
            await RevokeOverflowSessionsAsync(userId, utcNow);

            var session = Session.Create(
                id: sessionId,
                userId: userId,
                deviceInfo: deviceInfo,
                ttl: ttl,
                maxLifetime: maxLifetime,
                utcNow: utcNow
            );

            await _sessionRepository.SetAsync(session);

            return new SessionCreationResult(session, rawSession);
        }

        /// <summary>Operação para invalidar sessão.</summary>
        /// <param name="sessionId">Identificador da sessão.</param>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        public async Task InvalidateSessionAsync(string sessionId, DateTime utcNow)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
                return;

            var session = await _sessionRepository.GetAsync(sessionId);
            if (session is null)
                return;

            session.Revoke(utcNow);

            await _sessionRepository.SetAsync(session);
        }

        /// <summary>Operação para revogar sessões excedentes do usuário.</summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        private async Task RevokeOverflowSessionsAsync(Guid userId, DateTime utcNow)
        {
            var sessions = (await _sessionRepository.GetAllByUserIdAsync(userId))
                .Where(s => !s.IsRevoked() && !s.IsExpired(utcNow))
                .ToList();
            var sessionsToRevoke = _sessionPolicy.SelectSessionsToRevoke(sessions);

            foreach (var oldSession in sessionsToRevoke)
            {
                oldSession.Revoke(utcNow);
                await _sessionRepository.SetAsync(oldSession);
            }
        }
    }
}
