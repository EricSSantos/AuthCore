using AuthCore.Application.Models.Input;
using AuthCore.Application.Models.Output;
using AuthCore.Domain.Entities;
using AuthCore.Domain.Exceptions;
using AuthCore.Domain.Interfaces.Adapters.Http;
using AuthCore.Domain.Interfaces.Adapters.Security.Cripto;
using AuthCore.Domain.Interfaces.Adapters.Security.Tokens;
using AuthCore.Domain.Interfaces.Adapters.Sessions;
using AuthCore.Domain.Interfaces.Repositories;
using AuthCore.Domain.ValueObjects;

namespace AuthCore.Application.UseCases.AuthCase.SignIn
{
    public sealed class SigIn : ISigIn
    {
        private readonly IUserRepository _userRepository;
        private readonly ISessionAdapter _session;
        private readonly IAccessTokenAdapter _accessToken;
        private readonly IRefreshTokenAdapter _refreshToken;
        private readonly IBCryptAdapter _bCrypt;
        private readonly IDeviceAdapter _device;
        private readonly ICookieAdapter _cookie;

        public SigIn(
            IUserRepository userRepository,
            ISessionAdapter session,
            IAccessTokenAdapter accessToken,
            IRefreshTokenAdapter refreshToken,
            IBCryptAdapter bCrypt,
            IDeviceAdapter device,
            ICookieAdapter cookie)
        {
            _userRepository = userRepository;
            _session = session;
            _accessToken = accessToken;
            _refreshToken = refreshToken;
            _bCrypt = bCrypt;
            _device = device;
            _cookie = cookie;
        }

        public async Task<SignInViewModel> OnExecute(SigInInputModel input)
        {
            var user = await ValidateCredentials(input);
            var device = _device.GetDeviceInfo();
            var session = await _session.GetBySubAndDevice(user.Id, device);

            return await CreateSession(user.Id, session, device);
        }

        #region Private Methods

        private async Task<User> ValidateCredentials(SigInInputModel input)
        {
            var user = await _userRepository.GetByEmail(input.Email)
                ?? throw new InvalidCredentialsException();

            if (user.IsLocked())
                throw new DomainException(user.GetLockMessage()!);

            if (!_bCrypt.isValid(input.Password, user.Password))
            {
                user.RegisterFailedLogin();
                _userRepository.Update(user);
                await _userRepository.SaveChanges();
                throw new InvalidCredentialsException();
            }

            if (user.LoginAttempts.FailedAttempts > 0)
            {
                user.ResetLoginAttempts();
                _userRepository.Update(user);
                await _userRepository.SaveChanges();
            }

            return user;
        }

        private async Task<SignInViewModel> CreateSession(Guid userId, Session? session, DeviceInfo device)
        {
            // Se já existe uma sessão ativa para o mesmo usuário/dispositivo,
            // ela é removida antes de criar uma nova, garantindo que a sessão anterior
            // não permaneça ativa e evitando vulnerabilidades de "session fixation".
            if (session is not null)
                await _session.Delete(session.Id);

            var accessToken = _accessToken.GenerateAccessToken(userId);
            var (rawRefresh, hash) = _refreshToken.GenerateRefreshToken();
            var newSession = Session.Create(userId, device, hash);

            await _session.Add(newSession);

            _cookie.SetAuthCookies(newSession.Id, accessToken, rawRefresh);

            return SignInViewModel.ToViewModel(newSession.Id, accessToken, rawRefresh);
        }

        #endregion
    }
}
