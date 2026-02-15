using AuthCore.Domain.Aggregates.Sessions.Interfaces;
using AuthCore.Domain.Aggregates.Sessions.Results;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;

namespace AuthCore.Domain.Aggregates.Sessions
{
    /// <summary>Representa serviço de domínio de sessões.</summary>
    public sealed class SessionService : ISessionService
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly ISecureKeyGenerator _secureKeyGenerator;
        private readonly ISessionPolicy _sessionPolicy;

        #region Constructors

        /// <summary>Operação para criar instância de serviço de sessões.</summary>
        /// <param name="sessionRepository">Repositório de sessões.</param>
        /// <param name="secureKeyGenerator">Gerador de chaves seguras.</param>
        /// <param name="sessionPolicy">Política de sessões.</param>
        public SessionService(
            ISessionRepository sessionRepository,
            ISecureKeyGenerator secureKeyGenerator,
            ISessionPolicy sessionPolicy)
        {
            _sessionRepository = sessionRepository;
            _secureKeyGenerator = secureKeyGenerator;
            _sessionPolicy = sessionPolicy;
        }

        #endregion

        /// <summary>Operação para criar sessão.</summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="deviceInfo">Dados do dispositivo.</param>
        /// <param name="ttl">Tempo de expiração da sessão.</param>
        /// <param name="maxLifetime">Tempo máximo de vida útil.</param>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        public async Task<SessionCreationResult> CreateSessionAsync(Guid userId, DeviceInfo deviceInfo, TimeSpan ttl, TimeSpan maxLifetime, DateTime utcNow)
        {
            var sessions = (await _sessionRepository.GetAllByUserIdAsync(userId)).ToList();
            var sessionsToRevoke = _sessionPolicy.SelectSessionsToRevoke(sessions);

            foreach (var oldSession in sessionsToRevoke)
            {
                oldSession.Revoke(utcNow);
                await _sessionRepository.SetAsync(oldSession);
            }

            var rawSession = _secureKeyGenerator.Generate();
            var sessionHash = _secureKeyGenerator.Hash(rawSession);

            var session = Session.Create(
                id: sessionHash,
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
    }
}
