using AuthCore.Application.Services.Interfaces;
using AuthCore.Application.UseCases.AuthCase.Interfaces;
using AuthCore.Domain.Aggregates.SessionAggregate;
using AuthCore.Domain.Aggregates.UserAggregate;
using AuthCore.Domain.Commons.Exceptions;
using AuthCore.Domain.Commons.Interfaces.Http;
using AuthCore.Domain.Commons.Interfaces.Security;

namespace AuthCore.Application.UseCases.AuthCase
{
    public sealed class Refresh : IRefresh
    {
        private readonly IAccessToken _accessToken;
        private readonly IEntropy _entropy;
        private readonly ISessionRepository _sessionRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICookie _cookie;

        public Refresh(
            IAccessToken accessToken,
            IEntropy entropy,
            ISessionRepository sessionRepository,
            IUserRepository userRepository,
            ICookie cookie,
            IOwnership ownership)
        {
            _accessToken = accessToken;
            _entropy = entropy;
            _sessionRepository = sessionRepository;
            _userRepository = userRepository;
            _cookie = cookie;
        }

        public async Task OnExecute()
        {
            var rawSession = _cookie.Session;
            var rawRefresh = _cookie.RefreshToken;
            var oldSession = await ValidateSession(rawSession, rawRefresh);

            var user = await _userRepository.GetById(oldSession.UserId)
                ?? throw new NotFoundException();
            
            if (!user.IsActive())
                throw new ForbiddenException();

            var (newRawSession, newHashedSession) = _entropy.GeneratePair(32);
            var (newRawRefresh, newHashedRefresh) = _entropy.GeneratePair(64);

            var newSession = Session.Create(user.Id, oldSession.DeviceInfo, newHashedSession, newHashedRefresh);

            await _sessionRepository.Set(newSession);
            await _sessionRepository.Delete(oldSession.SessionHash);

            var newAccessToken = _accessToken.Generate(user.Id, user.Role);
            _cookie.SetAuthCookies(newRawSession, newAccessToken, newRawRefresh);
        }

        #region Private Methods

        private async Task<Session> ValidateSession(string rawSession, string rawRefresh)
        {
            var hashedSession = _entropy.Hash(rawSession);
            var session = await _sessionRepository.Get(hashedSession)
                ?? throw new NotFoundException();

            var sessionMatches = _entropy.Verify(rawSession, session.SessionHash);
            var refreshMatches = _entropy.Verify(rawRefresh, session.RefreshTokenHash);

            if (!sessionMatches || !refreshMatches || session.IsExpired())
            {
                await _sessionRepository.Delete(session.SessionHash);
                _cookie.RemoveAuthCookies();
                throw new ForbiddenException();
            }

            return session;
        }

        #endregion
    }
}
