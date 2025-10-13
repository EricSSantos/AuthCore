using AuthCore.Application.Services.Interfaces;
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
        private readonly IOwnership _ownership;

        public SignOut(
            IEntropy entropy,
            ISessionRepository sessionRepository,
            ICookie cookie,
            IOwnership ownership)
        {
            _entropy = entropy;
            _sessionRepository = sessionRepository;
            _cookie = cookie;
            _ownership = ownership;
        }

        public async Task OnExecute()
        {
            var rawSession = _cookie.Session;
            var hashedSession = _entropy.Hash(rawSession);

            var session = await _sessionRepository.Get(hashedSession)
                ?? throw new NotFoundException();

            var sessionMatches = _entropy.Verify(rawSession, session.SessionHash);

            if (!sessionMatches || session.IsExpired())
            {
                await _sessionRepository.Delete(session.SessionHash);
                _cookie.RemoveAuthCookies();
                throw new UnauthorizedException();
            }

            // Garante que a sessão pertence ao usuário autenticado
            // evitando exclusão indevida.
            _ownership.Ensure(session.UserId);

            await _sessionRepository.Delete(hashedSession);
            _cookie.RemoveAuthCookies();
        }
    }
}
