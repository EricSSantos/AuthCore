using AuthCore.Domain.Aggregates.Sessions;
using AuthCore.Domain.Aggregates.Sessions.Interfaces;
using AuthCore.Domain.Aggregates.Users;
using AuthCore.Domain.Aggregates.Users.Interfaces;
using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Web;
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

        /// <summary>Operação para criar instância do gerenciador de sessão do usuário.</summary>
        /// <param name="cookie">Serviço de cookies HTTP.</param>
        /// <param name="jwtTokenProvider">Provedor de token JWT.</param>
        /// <param name="sessionRepository">Repositório de sessões.</param>
        /// <param name="userRepository">Repositório de usuários.</param>
        /// <param name="secureKeyGenerator">Gerador de chaves seguras.</param>
        /// <param name="settings">Configurações de segurança.</param>
        /// <param name="logger">Serviço de logging.</param>
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
            Session session = await GetCurrentSession();

            User user = await _userRepository.GetByIdAsync(session.UserId)
                ?? throw new UnauthorizedException();

            if (!user.IsActiveAndVerified())
            {
                ClearCookies();
                await _sessionRepository.DeleteAsync(session.Id);
                throw new UnauthorizedException();
            }

            return user;
        }

        /// <summary>Operação para obter sessão atual validada.</summary>
        public async Task<Session> GetCurrentSession()
        {
            string hashedSession = GetHashedSessionOrThrow();

            Session session = await _sessionRepository.GetAsync(hashedSession)
                ?? throw new UnauthorizedException();

            EnsureOwnership(session.UserId);

            if (session.IsRevoked())
            {
                ClearCookies();
                await _sessionRepository.DeleteAsync(session.Id);
                throw new UnauthorizedException();
            }

            if (session.IsExpired(DateTime.UtcNow))
            {
                ClearCookies();
                await _sessionRepository.DeleteAsync(session.Id);
                throw new UnauthorizedException();
            }

            return session;
        }

        /// <summary>Operação para obter usuário e sessão atuais.</summary>
        public async Task<(User User, Session Session)> GetCurrentUserFromSession()
        {
            string hashedSession = GetHashedSessionOrThrow();

            Session session = await _sessionRepository.GetAsync(hashedSession)
                ?? throw new UnauthorizedException();

            if (session.IsRevoked())
            {
                _logger.LogWarning("Sessão revogada detectada. SessionId={SessionId}.", session.Id);
                ClearCookies();
                await _sessionRepository.DeleteAsync(session.Id);
                throw new UnauthorizedException();
            }

            if (session.IsExpired(DateTime.UtcNow))
            {
                _logger.LogWarning("Sessão expirada detectada. SessionId={SessionId}.", session.Id);
                ClearCookies();
                await _sessionRepository.DeleteAsync(session.Id);
                throw new UnauthorizedException();
            }

            User user = await _userRepository.GetByIdAsync(session.UserId)
                ?? throw new UnauthorizedException();

            if (!user.IsActiveAndVerified())
            {
                _logger.LogWarning("Sessão inválida para usuário {UserId}. Usuário inativo ou não verificado.", user.Id);
                ClearCookies();
                await _sessionRepository.DeleteAsync(session.Id);
                throw new UnauthorizedException();
            }

            return (user, session);
        }

        /// <summary>Operação para obter sessões ativas do usuário.</summary>
        public async Task<IReadOnlyCollection<Session>> GetActiveSessions()
        {
            Session currentSession = await GetCurrentSession();

            List<Session> sessions = (await _sessionRepository.GetAllByUserIdAsync(currentSession.UserId))
                .ToList();

            List<Session> activeSessions = sessions
                .Where(session => !session.IsExpired(DateTime.UtcNow) && !session.IsRevoked())
                .ToList();

            if (!activeSessions.Any())
                throw new UnauthorizedException();

            return activeSessions;
        }

        /// <summary>Operação para definir cookies de sessão e token.</summary>
        /// <param name="rawSession">Identificador bruto de sessão.</param>
        /// <param name="accessToken">Token de acesso.</param>
        public void SetCookies(string rawSession, string accessToken)
        {
            if (string.IsNullOrWhiteSpace(rawSession) || string.IsNullOrWhiteSpace(accessToken))
                throw new UnauthorizedException();

            TimeSpan sessionTtl = TimeSpan.FromDays(_settings.Session.ExpiresInDays);
            TimeSpan accessTtl = TimeSpan.FromMinutes(_settings.Jwt.ExpiresInMinutes);
            TimeSpan csrfTtl = sessionTtl;
            string csrfToken = _secureKeyGenerator.Generate(32);

            _cookie.Set(_settings.Cookies.SessionKey, rawSession, sessionTtl);
            _cookie.Set(_settings.Cookies.AccessTokenKey, accessToken, accessTtl);
            _cookie.Set(_settings.Cookies.CsrfKey, csrfToken, csrfTtl, httpOnly: false);
            _logger.LogInformation("Cookies de sessão definidos com sucesso.");
        }

        /// <summary>Operação para remover cookies de sessão e token.</summary>
        public void ClearCookies()
        {
            _cookie.Remove(_settings.Cookies.SessionKey);
            _cookie.Remove(_settings.Cookies.AccessTokenKey);
            _cookie.Remove(_settings.Cookies.CsrfKey);
            _logger.LogInformation("Cookies de sessão removidos.");
        }

        /// <summary>Operação para obter hash da sessão atual com tratamento de formato inválido.</summary>
        private string GetHashedSessionOrThrow()
        {
            try
            {
                return _secureKeyGenerator.Hash(Session);
            }
            catch (FormatException ex)
            {
                _logger.LogWarning(ex, "Cookie de sessão com formato inválido.");
                ClearCookies();
                throw new UnauthorizedException();
            }
        }

        /// <summary>Operação para validar se o token pertence ao usuário da sessão.</summary>
        /// <param name="sessionUserId">Identificador do usuário da sessão.</param>
        private void EnsureOwnership(Guid sessionUserId)
        {
            Guid tokenUserId = _jwtTokenProvider.Sub;

            if (sessionUserId != tokenUserId)
            {
                _logger.LogWarning(
                    "Token não corresponde ao usuário da sessão. SessionUserId={SessionUserId}, TokenUserId={TokenUserId}.",
                    sessionUserId,
                    tokenUserId);

                throw new UnauthorizedException();
            }
        }
    }
}
