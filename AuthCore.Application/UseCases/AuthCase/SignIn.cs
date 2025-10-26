using AuthCore.Application.Models.Input;
using AuthCore.Application.UseCases.AuthCase.Interfaces;
using AuthCore.Domain.Aggregates.SessionAggregate;
using AuthCore.Domain.Aggregates.UserAggregate;
using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Http;
using AuthCore.Domain.Core.Interfaces.Security;
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

        public async Task OnExecute(SignInInputModel input)
        {
            var user = await _userRepository.GetByEmail(input.Email)
                ?? throw new UnauthorizedException("E-mail ou senha inválidos.");

            await ValidateUserCredentials(user, input.Password);
            await EnforceSessionLimit(user.Id);

            var session = _secureKeyGenerator.Generate();
            var sessionHash = _secureKeyGenerator.Hash(session);

            var ttl = TimeSpan.FromDays(_settings.Session.ExpiresInDays);
            var maxLifetime = TimeSpan.FromDays(_settings.Session.MaxLifetimeInDays);

            var newSession = Session.Create(
                id:             sessionHash,
                userId:         user.Id,
                deviceInfo:     _device.Device,
                ttl:            ttl,
                maxLifetime:    maxLifetime
            );

            await _sessionRepository.Set(newSession);

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

                _userRepository.Update(user);
                await _userRepository.SaveChanges();

                throw new UnauthorizedException("E-mail ou senha inválidos.");
            }

            if (user.LoginAttempts.FailedAttempts > 0)
            {
                user.LoginAttempts.Reset();
                _userRepository.Update(user);
                await _userRepository.SaveChanges();
            }
        }

        private async Task EnforceSessionLimit(Guid userId)
        {
            var sessions = await _sessionRepository.GetAllByUserId(userId);
            if (sessions.Count() < 4)
                return;

            var oldestSession = sessions
                .OrderBy(s => s.CreatedAt)
                .First();

            await _sessionRepository.Delete(oldestSession.Id);
        }

        #endregion
    }
}
