using AuthCore.Application.Models.Requests;
using AuthCore.Application.UseCases.Auth.Interfaces;
using AuthCore.Domain.Aggregates.Sessions.Interfaces;
using AuthCore.Domain.Aggregates.Users.Interfaces;
using AuthCore.Domain.Core.ValueObjects;
using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Web;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;
using AuthCore.Domain.Core.Settings;
using Microsoft.Extensions.Logging;

namespace AuthCore.Application.UseCases.Auth
{
    /// <summary>Representa caso de uso de autenticação.</summary>
    public sealed class SignIn : ISignIn
    {
        private readonly IUserRepository _userRepository;
        private readonly ISessionService _sessionService;
        private readonly IJwtTokenProvider _jwtTokenProvider;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ISecureKeyGenerator _secureKeyGenerator;
        private readonly IUserAuthenticationPolicy _authenticationPolicy;
        private readonly ISessionState _sessionState;
        private readonly IDevice _device;
        private readonly IRateLimitStore _rateLimit;
        private readonly SecuritySettings _settings;
        private readonly ILogger<SignIn> _logger;

        /// <summary>Operação para criar instância de caso de uso.</summary>
        /// <param name="userRepository">Repositório de usuários.</param>
        /// <param name="sessionService">Serviço de domínio de sessões.</param>
        /// <param name="jwtTokenProvider">Provedor de token JWT.</param>
        /// <param name="passwordHasher">Serviço de hash de senhas.</param>
        /// <param name="secureKeyGenerator">Gerador seguro de chaves.</param>
        /// <param name="authenticationPolicy">Política de autenticação do usuário.</param>
        /// <param name="sessionState">Gerenciador de estado da sessão atual.</param>
        /// <param name="device">Serviço de identificação de dispositivo.</param>
        /// <param name="rateLimit">Armazenamento de limitação de taxa.</param>
        /// <param name="settings">Configurações de segurança.</param>
        /// <param name="logger">Serviço de logging da operação.</param>
        public SignIn(
            IUserRepository userRepository,
            ISessionService sessionService,
            IJwtTokenProvider jwtTokenProvider,
            IPasswordHasher passwordHasher,
            ISecureKeyGenerator secureKeyGenerator,
            IUserAuthenticationPolicy authenticationPolicy,
            ISessionState sessionState,
            IDevice device,
            IRateLimitStore rateLimit,
            SecuritySettings settings,
            ILogger<SignIn> logger)
        {
            _userRepository = userRepository;
            _sessionService = sessionService;
            _jwtTokenProvider = jwtTokenProvider;
            _passwordHasher = passwordHasher;
            _secureKeyGenerator = secureKeyGenerator;
            _authenticationPolicy = authenticationPolicy;
            _sessionState = sessionState;
            _device = device;
            _rateLimit = rateLimit;
            _settings = settings;
            _logger = logger;
        }

        /// <summary>Operação para realizar o processo de autenticação.</summary>
        /// <param name="request">Credenciais do usuário.</param>
        public async Task OnExecuteAsync(SignInRequest request)
        {
            var deviceInfo = _device.Get();
            var ip = deviceInfo.Ip;
            var emailKey = Email.Normalize(request.Email);
            if (_settings.Abuse.SignInIpPerMinute > 0)
            {
                await _rateLimit.EnsureFixedWindowAsync(
                    key: $"auth:sign-in:ip:{ip}",
                    limit: _settings.Abuse.SignInIpPerMinute,
                    window: TimeSpan.FromMinutes(1),
                    errorMessage: "Muitas tentativas de login. Tente novamente mais tarde.");
            }

            if (_settings.Abuse.SignInEmailPerMinute > 0)
            {
                await _rateLimit.EnsureFixedWindowAsync(
                    key: $"auth:sign-in:email:{emailKey}",
                    limit: _settings.Abuse.SignInEmailPerMinute,
                    window: TimeSpan.FromMinutes(1),
                    errorMessage: "Muitas tentativas de login. Tente novamente mais tarde.");
            }

            // Recupera o usuário sem revelar se o e-mail existe
            var user = await _userRepository.GetByEmailAsync(request.Email)
                ?? throw new InvalidCredentialsException();

            var utcNow = DateTime.UtcNow;
            var passwordValid = _passwordHasher.IsValid(request.Password, user.Password.Value);

            try
            {
                user.Authenticate(passwordValid, _authenticationPolicy, utcNow);
            }
            catch (UnauthorizedException)
            {
                _logger.LogWarning("Falha de autenticação para usuário {UserId}.", user.Id);
                await _userRepository.UpdateAsync(user);
                throw;
            }

            await _userRepository.UpdateAsync(user);

            var rawSession = _secureKeyGenerator.Generate();
            var sessionId = _secureKeyGenerator.Hash(rawSession);

            // Criação da sessão delegada ao domínio de sessão
            var sessionResult = await _sessionService.CreateSessionAsync(
                user.Id,
                sessionId,
                rawSession,
                deviceInfo,
                TimeSpan.FromDays(_settings.Session.ExpiresInDays),
                TimeSpan.FromDays(_settings.Session.MaxLifetimeInDays),
                utcNow
            );

            // Gera um novo access token vinculado ao usuário autenticado
            var accessToken = _jwtTokenProvider.Generate(user.Id);

            // Define cookies usando a sessão criada e o novo token
            _sessionState.SetCookies(sessionResult.RawSession, accessToken);
            _logger.LogInformation("Login realizado para usuário {UserId}.", user.Id);
        }
    }
}
