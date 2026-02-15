using AuthCore.Domain.Aggregates.Sessions;
using AuthCore.Domain.Aggregates.Sessions.Interfaces;
using AuthCore.Domain.Aggregates.Users;
using AuthCore.Domain.Aggregates.Users.Interfaces;
using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Web;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;
using AuthCore.Domain.Core.Settings;
using Microsoft.Extensions.Logging;

namespace AuthCore.Infrastructure.Security
{
    /// <summary>Representa gerenciador de sessão do usuário.</summary>
    public sealed class SessionState : ISessionState
    {
        private readonly ICookie _cookie;
        private readonly IJwtTokenProvider _jwtTokenProvider;
        private readonly ISessionRepository _sessionRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISecureKeyGenerator _secureKeyGenerator;
        private readonly SecuritySettings _settings;
        private readonly ILogger<SessionState> _logger;

        public SessionState(
            ICookie cookie,
            IJwtTokenProvider jwtTokenProvider,
            ISessionRepository sessionRepository,
            IUserRepository userRepository,
            ISecureKeyGenerator secureKeyGenerator,
            SecuritySettings settings,
            ILogger<SessionState> logger)
        {
            _cookie = cookie;
            _jwtTokenProvider = jwtTokenProvider;
            _sessionRepository = sessionRepository;
            _userRepository = userRepository;
            _secureKeyGenerator = secureKeyGenerator;
            _settings = settings;
            _logger = logger;
        }

        /// <summary>Operação para obter identificador bruto de sessão do cookie.</summary>
        public string Session
        {
            get { return _cookie.Get(_settings.Cookies.SessionKey); }
        }

        /// <summary>Operação para obter usuário atual a partir da sessão.</summary>
        public async Task<User> GetCurrentUser()
        {
            var session = await GetCurrentSession();

            var user = await _userRepository.GetByIdAsync(session.UserId)
                ?? throw new NotFoundException("Usuário não encontrado.");

            if (!user.IsActiveAndVerified())
            {
                ClearCookies();
                await _sessionRepository.DeleteAsync(session.Id);
                throw new ForbiddenException("Usuário inativo ou não verificado.");
            }

            return user;
        }

        /// <summary>Operação para obter sessão atual validada.</summary>
        public async Task<Session> GetCurrentSession()
        {
            var session = await _sessionRepository.GetAsync(_secureKeyGenerator.Hash(Session))
                ?? throw new NotFoundException("Sessão não encontrada.");

            EnsureOwnership(session.UserId);

            if (session.IsRevoked())
            {
                ClearCookies();
                await _sessionRepository.DeleteAsync(session.Id);
                throw new ForbiddenException("Sessão revogada.");
            }

            if (session.IsExpired(DateTime.UtcNow))
            {
                ClearCookies();
                await _sessionRepository.DeleteAsync(session.Id);
                throw new ForbiddenException("Sessão expirada.");
            }

            return session;
        }

        /// <summary>Operação para obter usuário e sessão atuais.</summary>
        public async Task<(User User, Session Session)> GetCurrentUserFromSession()
        {
            var session = await _sessionRepository.GetAsync(_secureKeyGenerator.Hash(Session))
                ?? throw new NotFoundException("Sessão não encontrada.");

            if (session.IsRevoked())
            {
                _logger.LogWarning("Sessão revogada detectada. SessionId={SessionId}.", session.Id);
                ClearCookies();
                await _sessionRepository.DeleteAsync(session.Id);
                throw new ForbiddenException("Sessão revogada.");
            }

            if (session.IsExpired(DateTime.UtcNow))
            {
                _logger.LogWarning("Sessão expirada detectada. SessionId={SessionId}.", session.Id);
                ClearCookies();
                await _sessionRepository.DeleteAsync(session.Id);
                throw new ForbiddenException("Sessão expirada.");
            }

            var user = await _userRepository.GetByIdAsync(session.UserId)
                ?? throw new NotFoundException("Usuário não encontrado.");

            if (!user.IsActiveAndVerified())
            {
                _logger.LogWarning("Sessão inválida para usuário {UserId}. Usuário inativo ou não verificado.", user.Id);
                ClearCookies();
                await _sessionRepository.DeleteAsync(session.Id);
                throw new ForbiddenException("Usuário inativo ou não verificado.");
            }

            return (user, session);
        }

        /// <summary>Operação para obter sessões ativas do usuário.</summary>
        public async Task<IReadOnlyCollection<Session>> GetActiveSessions()
        {
            var current = await GetCurrentSession();

            var sessions = (await _sessionRepository.GetAllByUserIdAsync(current.UserId))
                .ToList();

            var activeSessions = sessions
                .Where(s => !s.IsExpired(DateTime.UtcNow) && !s.IsRevoked())
                .ToList();

            if (!activeSessions.Any())
                throw new NotFoundException("Nenhuma sessão encontrada.");

            return activeSessions;
        }

        /// <summary>Operação para definir cookies de sessão e token.</summary>
        /// <param name="rawSession">Identificador bruto de sessão.</param>
        /// <param name="accessToken">Token de acesso.</param>
        public void SetCookies(string rawSession, string accessToken)
        {
            if (string.IsNullOrWhiteSpace(rawSession) || string.IsNullOrWhiteSpace(accessToken))
                throw new BadRequestException("Sessão ou token inválido.");

            var sessionTtl = TimeSpan.FromDays(_settings.Session.ExpiresInDays);
            var accessTtl = TimeSpan.FromMinutes(_settings.Jwt.ExpiresInMinutes);

            _cookie.Set(_settings.Cookies.SessionKey, rawSession, sessionTtl);
            _cookie.Set(_settings.Cookies.AccessTokenKey, accessToken, accessTtl);
            _logger.LogInformation("Cookies de sessão definidos com sucesso.");
        }

        /// <summary>Operação para remover cookies de sessão e token.</summary>
        public void ClearCookies()
        {
            _cookie.Remove(_settings.Cookies.SessionKey);
            _cookie.Remove(_settings.Cookies.AccessTokenKey);
            _logger.LogInformation("Cookies de sessão removidos.");
        }
        #region Helperes

        /// <summary>Operação para validar se o token pertence ao usuário da sessão.</summary>
        /// <param name="sessionUserId">Identificador do usuário da sessão.</param>
        private void EnsureOwnership(Guid sessionUserId)
        {
            var tokenUserId = _jwtTokenProvider.Sub;
            if (sessionUserId != tokenUserId)
            {
                _logger.LogWarning("Token não corresponde ao usuário da sessão. SessionUserId={SessionUserId}, TokenUserId={TokenUserId}.", sessionUserId, tokenUserId);
                throw new ForbiddenException("Token de acesso não corresponde ao usuário da sessão.");
            }
        }

        #endregion
    }
}
