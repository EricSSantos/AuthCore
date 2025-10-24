using AuthCore.Application.Models.Input;
using AuthCore.Application.UseCases.AuthCase.Interfaces;
using AuthCore.Domain.Aggregates.SessionAggregate;
using AuthCore.Domain.Aggregates.UserAggregate;
using AuthCore.Domain.Commons.Exceptions;
using AuthCore.Domain.Commons.Interfaces.Http;
using AuthCore.Domain.Commons.Interfaces.Security.Hashing;
using AuthCore.Domain.Commons.Interfaces.Security.Jwt;

namespace AuthCore.Application.UseCases.AuthCase
{
    public sealed class SignIn : ISignIn
    {
        private readonly IUserRepository _userRepository;
        private readonly ISessionRepository _sessionRepository;
        private readonly IAccessToken _accessToken;
        private readonly IEntropy _entropy;
        private readonly IBCrypt _bCrypt;
        private readonly IDevice _device;
        private readonly ICookie _cookie;

        public SignIn(
            IUserRepository userRepository,
            ISessionRepository sessionRepository,
            IAccessToken accessToken,
            IEntropy entropy,
            IBCrypt bCrypt,
            IDevice device,
            ICookie cookie)
        {
            _userRepository = userRepository;
            _sessionRepository = sessionRepository;
            _accessToken = accessToken;
            _entropy = entropy;
            _bCrypt = bCrypt;
            _device = device;
            _cookie = cookie;
        }

        public async Task OnExecute(SignInInputModel input)
        {
            var user = await ValidateCredentials(input.Email, input.Password);
            await ValidateSessions(user.Id);

            var (rawSession, hashedSession) = _entropy.GeneratePair(32);
            var (rawRefresh, hashedRefresh) = _entropy.GeneratePair(64);

            var device = _device.Device;

            var session = Session.Create(user.Id, device, hashedSession, hashedRefresh);
            await _sessionRepository.Set(session);

            var accessToken = _accessToken.Generate(user.Id, user.Role);

            _cookie.SetAuthCookies(rawSession, accessToken, rawRefresh);
        }

        #region Private Methods

        private async Task<User> ValidateCredentials(string email, string password)
        {
            var user = await _userRepository.GetByEmail(email)
                ?? throw new UnauthorizedException("E-mail ou senha inválidos.");

            user.SignIn(_bCrypt.isValid(password, user.Password.Value));

            _userRepository.Update(user);
            await _userRepository.SaveChanges();

            return user;
        }

        private async Task ValidateSessions(Guid userId)
        {
            var sessions = (await _sessionRepository.GetByUserId(userId)).ToList();

            if (sessions.Count < 4)
                return;

            var oldestSession = sessions.Last();

            await _sessionRepository.DeleteById(oldestSession.Id);
        }

        #endregion
    }
}
