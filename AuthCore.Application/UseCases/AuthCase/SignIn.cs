using AuthCore.Application.Models.Requests;
using AuthCore.Application.UseCases.AuthCase.Interfaces;
using AuthCore.Domain.Aggregates.SessionAggregate;
using AuthCore.Domain.Aggregates.SessionAggregate.Interfaces;
using AuthCore.Domain.Aggregates.UserAggregate;
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
        private readonly ISessionRepository _sessionRepository;
        private readonly IJwtTokenProvider _jwtTokenProvider;
        private readonly ISecureKeyGenerator _secureKeyGenerator;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ISessionState _sessionState;
        private readonly IDevice _device;
        private readonly SecuritySettings _settings;

        public SignIn(
            IUserRepository userRepository,
            ISessionRepository sessionRepository,
            IJwtTokenProvider jwtTokenProvider,
            ISecureKeyGenerator secureKeyGenerator,
            IPasswordHasher passwordHasher,
            ISessionState sessionState,
            IDevice device,
            SecuritySettings settings)
        {
            _userRepository = userRepository;
            _sessionRepository = sessionRepository;
            _jwtTokenProvider = jwtTokenProvider;
            _secureKeyGenerator = secureKeyGenerator;
            _passwordHasher = passwordHasher;
            _sessionState = sessionState;
            _device = device;
            _settings = settings;
        }

        public async Task OnExecuteAsync(SignInRequest request)
        {
            var user = await _userRepository.GetByEmailAsync(request.Email)
                ?? throw new UnauthorizedException("E-mail ou senha inválidos.");

            await ValidateUserCredentials(user, request.Password);
            await EnforceSessionLimit(user.Id);

            var session = _secureKeyGenerator.Generate();
            var sessionHash = _secureKeyGenerator.Hash(session);

            var ttl = TimeSpan.FromDays(_settings.Session.ExpiresInDays);
            var maxLifetime = TimeSpan.FromDays(_settings.Session.MaxLifetimeInDays);

            var newSession = Session.Create(
                id: sessionHash,
                userId: user.Id,
                deviceInfo: _device.Device,
                ttl: ttl,
                maxLifetime: maxLifetime
            );

            await _sessionRepository.SetAsync(newSession);

            var accessToken = _jwtTokenProvider.Generate(user.Id);
            _sessionState.SetCookies(session, accessToken);
        }

        #region Helpers

        private async Task ValidateUserCredentials(User user, string password)
        {
            if (!user.IsActive())
                throw new ForbiddenException("Usuário inativo.");

            if (!_passwordHasher.IsValid(password, user.Password.Value))
            {
                user.LoginAttempts.RegisterFailure();

                await _userRepository.UpdateAsync(user);

                throw new UnauthorizedException("E-mail ou senha inválidos.");
            }

            if (user.LoginAttempts.FailedAttempts > 0)
            {
                user.LoginAttempts.Reset();
                await _userRepository.UpdateAsync(user);
            }
        }

        private async Task EnforceSessionLimit(Guid userId)
        {
            var sessions = await _sessionRepository.GetAllByUserIdAsync(userId);
            if (sessions.Count() < 4)
                return;

            var oldestSession = sessions
                .OrderBy(s => s.CreatedAt)
                .First();

            await _sessionRepository.DeleteAsync(oldestSession.Id);
        }

        #endregion
    }
}
