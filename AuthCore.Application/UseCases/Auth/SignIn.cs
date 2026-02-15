using AuthCore.Application.Models.Requests;
using AuthCore.Application.UseCases.Auth.Interfaces;
using AuthCore.Domain.Aggregates.Sessions.Interfaces;
using AuthCore.Domain.Aggregates.Users.Interfaces;
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
        private readonly IUserAuthenticationPolicy _authenticationPolicy;
        private readonly ISessionState _sessionState;
        private readonly IDevice _device;
        private readonly SecuritySettings _settings;
        private readonly ILogger<SignIn> _logger;

        public SignIn(
            IUserRepository userRepository,
            ISessionService sessionService,
            IJwtTokenProvider jwtTokenProvider,
            IPasswordHasher passwordHasher,
            IUserAuthenticationPolicy authenticationPolicy,
            ISessionState sessionState,
            IDevice device,
            SecuritySettings settings,
            ILogger<SignIn> logger)
        {
            _userRepository = userRepository;
            _sessionService = sessionService;
            _jwtTokenProvider = jwtTokenProvider;
            _passwordHasher = passwordHasher;
            _authenticationPolicy = authenticationPolicy;
            _sessionState = sessionState;
            _device = device;
            _settings = settings;
            _logger = logger;
        }

        public async Task OnExecuteAsync(SignInRequest request)
        {
            // Recupera o usuário sem revelar se o e-mail existe
            var user = await _userRepository.GetByEmailAsync(request.Email)
                ?? throw new UnauthorizedException("E-mail ou senha inválidos.");

            var utcNow = DateTime.UtcNow;

            try
            {
                user.Authenticate(request.Password, _passwordHasher, _authenticationPolicy, utcNow);
            }
            catch (UnauthorizedException)
            {
                _logger.LogWarning("Falha de autenticação para e-mail {Email}.", request.Email);
                await _userRepository.UpdateAsync(user);
                throw;
            }

            await _userRepository.UpdateAsync(user);

            // Criação da sessão delegada ao domínio de sessão
            var sessionResult = await _sessionService.CreateSessionAsync(
                user.Id,
                _device.Device,
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
