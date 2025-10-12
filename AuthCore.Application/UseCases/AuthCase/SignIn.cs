using AuthCore.Application.Models.Input;
using AuthCore.Application.UseCases.AuthCase.Interfaces;
using AuthCore.Domain.Aggregates.SessionAggregate;
using AuthCore.Domain.Aggregates.UserAggregate;
using AuthCore.Domain.Commons.Exceptions;
using AuthCore.Domain.Commons.Interfaces.Http;
using AuthCore.Domain.Commons.Interfaces.Security;

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
            var user = await _userRepository.GetByEmail(input.Email)
                ?? throw new UnauthorizedException();

            user.ValidateSignIn(_bCrypt.isValid(input.Password, user.Password));

            _userRepository.Update(user);
            await _userRepository.SaveChanges();

            var (rawSession, hashedSession) = _entropy.GeneratePair(32);
            var (rawRefresh, hashedRefresh) = _entropy.GeneratePair(64);

            var device = _device.Device;

            var session = Session.Create(user.Id, device, hashedSession, hashedRefresh);
            await _sessionRepository.Set(session);

            var accessToken = _accessToken.Generate(user.Id, user.Role);

            _cookie.SetAuthCookies(rawSession, accessToken, rawRefresh);
        }
    }
}
