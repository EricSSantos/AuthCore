using AuthCore.Application.UseCases.AuthCase.Interfaces;
using AuthCore.Domain.Aggregates.SessionAggregate.Interfaces;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;

namespace AuthCore.Application.UseCases.AuthCase
{
    public sealed class SignOut : ISignOut
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly ISessionState _sessionState;

        public SignOut(
            ISessionRepository sessionRepository,
            ISessionState sessionState)
        {
            _sessionRepository = sessionRepository;
            _sessionState = sessionState;
        }

        public async Task OnExecute()
        {
            var session = await _sessionState.GetCurrentSession();
            await _sessionRepository.Delete(session.Id);
            _sessionState.ClearCookies();
        }
    }
}
