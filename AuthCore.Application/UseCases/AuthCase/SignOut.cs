using AuthCore.Application.UseCases.AuthCase.Interfaces;
using AuthCore.Domain.Aggregates.SessionAggregate;
using AuthCore.Domain.Commons.Exceptions;
using AuthCore.Domain.Commons.Interfaces.Http;
using AuthCore.Domain.Commons.Interfaces.Security;

namespace AuthCore.Application.UseCases.AuthCase
{
    public sealed class SignOut : ISignOut
    {
        private readonly IEntropy _entropy;
        private readonly ISessionRepository _sessionRepository;
        private readonly ICookie _cookie;
        private readonly IAccessToken _accessToken;

        public SignOut(
            IEntropy entropy,
            ISessionRepository sessionRepository,
            ICookie cookie,
            IAccessToken accessToken)
        {
            _entropy = entropy;
            _sessionRepository = sessionRepository;
            _cookie = cookie;
            _accessToken = accessToken;
        }

        public async Task OnExecute()
        {
            var userId = _accessToken.Sub;
            var rawSession = _cookie.Session;
            var hashedSession = _entropy.Hash(rawSession);

            var session = await _sessionRepository.Get(hashedSession);

            var matching = session != null && _entropy.Verify(rawSession, session.SessionHash) && session.UserId == userId;

            if (!matching || session!.IsExpired())
            {
                if (session != null)
                    await _sessionRepository.Delete(session.SessionHash);

                _cookie.RemoveAuthCookies();
                throw new UnauthorizedException("Sessão inválida ou expirada.");
            }

            await _sessionRepository.Delete(hashedSession);
            _cookie.RemoveAuthCookies();
        }
    }
}
