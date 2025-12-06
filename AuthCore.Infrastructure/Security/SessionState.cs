using AuthCore.Domain.Aggregates.SessionAggregate;
using AuthCore.Domain.Aggregates.SessionAggregate.Interfaces;
using AuthCore.Domain.Aggregates.UserAggregate;
using AuthCore.Domain.Aggregates.UserAggregate.Interfaces;
using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Http;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;
using AuthCore.Domain.Core.Settings;

namespace AuthCore.Infrastructure.Security
{
    public sealed class SessionState : ISessionState
    {
        #region Constants

        public const string SESSION_ID = "session";
        public const string ACCESS_TOKEN = "access_token";

        #endregion

        private readonly ICookie _cookie;
        private readonly IJwtTokenProvider _jwtTokenProvider;
        private readonly ISessionRepository _sessionRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISecureKeyGenerator _secureKeyGenerator;
        private readonly SecuritySettings _settings;

        public SessionState(
            ICookie cookie,
            IJwtTokenProvider jwtTokenProvider,
            ISessionRepository sessionRepository,
            IUserRepository userRepository,
            ISecureKeyGenerator secureKeyGenerator,
            SecuritySettings settings)
        {
            _cookie = cookie;
            _jwtTokenProvider = jwtTokenProvider;
            _sessionRepository = sessionRepository;
            _userRepository = userRepository;
            _secureKeyGenerator = secureKeyGenerator;
            _settings = settings;
        }

        public string Session
        {
            get { return _cookie.Get(SESSION_ID); }
        }

        public async Task<User> GetCurrentUser()
        {
            var session = await GetCurrentSession();

            var user = await _userRepository.GetById(session.UserId)
                ?? throw new NotFoundException("Usuário não encontrado.");

            if (!user.IsActive())
            {
                ClearCookies();
                throw new ForbiddenException("Usuário inativo.");
            }

            return user;
        }

        public async Task<Session> GetCurrentSession()
        {
            var session = await _sessionRepository.GetAsync(_secureKeyGenerator.Hash(Session))
                ?? throw new NotFoundException("Sessão não encontrada.");

            EnsureOwnership(session.UserId);

            if (session.IsExpired())
            {
                ClearCookies();
                await _sessionRepository.DeleteAsync(session.Id);
                throw new ForbiddenException("Sessão expirada.");
            }

            return session;
        }

        public async Task<IReadOnlyCollection<Session>> GetOtherSessions()
        {
            var current = await GetCurrentSession();

            var sessions = (await _sessionRepository.GetAllByUserIdAsync(current.UserId))
                .Where(s => s.Id != current.Id)
                .ToList();

            var activeSessions = sessions
                .Where(s => !s.IsExpired())
                .ToList();

            if (activeSessions.Count == 0)
                throw new NotFoundException("Nenhuma sessão encontrada.");

            return activeSessions;
        }

        public void SetCookies(string rawSession, string accessToken)
        {
            if (string.IsNullOrWhiteSpace(rawSession) || string.IsNullOrWhiteSpace(accessToken))
                throw new BadRequestException("Sessão ou token inválido.");

            var sessionTtl = TimeSpan.FromDays(_settings.Session.ExpiresInDays);
            var accessTtl = TimeSpan.FromMinutes(_settings.Jwt.ExpiresInMinutes);

            _cookie.Set(SESSION_ID, rawSession, sessionTtl);
            _cookie.Set(ACCESS_TOKEN, accessToken, accessTtl);
        }

        public void ClearCookies()
        {
            _cookie.Remove(SESSION_ID);
            _cookie.Remove(ACCESS_TOKEN);
        }

        #region Helperes

        private void EnsureOwnership(Guid sessionUserId)
        {
            var tokenUserId = _jwtTokenProvider.Sub;
            if (sessionUserId != tokenUserId)
                throw new ForbiddenException("Token de acesso não corresponde ao usuário da sessão.");
        }

        #endregion
    }
}
