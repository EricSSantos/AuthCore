using AuthCore.Application.Models.Requests;
using AuthCore.Application.UseCases.AuthCase.Interfaces;
using AuthCore.Domain.Aggregates.SessionAggregate.Interfaces;
using AuthCore.Domain.Aggregates.UserAggregate.Interfaces;
using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Http;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;
using AuthCore.Domain.Core.Settings;

namespace AuthCore.Application.UseCases.AuthCase
{
    public sealed class SignIn : ISignIn
    {
        private readonly IUserRepository _userRepository;
        private readonly ISessionService _sessionService;
        private readonly IJwtTokenProvider _jwtTokenProvider;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ISessionState _sessionState;
        private readonly IDevice _device;
        private readonly SecuritySettings _settings;

        public SignIn(
            IUserRepository userRepository,
            ISessionService sessionService,
            IJwtTokenProvider jwtTokenProvider,
            IPasswordHasher passwordHasher,
            ISessionState sessionState,
            IDevice device,
            SecuritySettings settings)
        {
            _userRepository = userRepository;
            _sessionService = sessionService;
            _jwtTokenProvider = jwtTokenProvider;
            _passwordHasher = passwordHasher;
            _sessionState = sessionState;
            _device = device;
            _settings = settings;
        }

        public async Task OnExecuteAsync(SignInRequest request)
        {
            // Recupera o usuário sem revelar se o e-mail existe
            var user = await _userRepository.GetByEmailAsync(request.Email)
                ?? throw new UnauthorizedException("E-mail ou senha inválidos.");

            try
            {
                user.Authenticate(request.Password, _passwordHasher);
            }
            catch (UnauthorizedException)
            {
                await _userRepository.UpdateAsync(user);
                throw;
            }

            await _userRepository.UpdateAsync(user);

            // Criação da sessão delegada ao domínio de sessão
            var sessionResult = await _sessionService.CreateSessionAsync(
                user.Id,
                _device.Device,
                TimeSpan.FromDays(_settings.Session.ExpiresInDays),
                TimeSpan.FromDays(_settings.Session.MaxLifetimeInDays)
            );

            // Gera um novo access token vinculado ao usuário autenticado
            var accessToken = _jwtTokenProvider.Generate(user.Id);

            // Define cookies usando a sessão criada e o novo token
            _sessionState.SetCookies(sessionResult.RawSession, accessToken);
        }
    }
}
