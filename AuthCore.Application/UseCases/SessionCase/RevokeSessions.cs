using AuthCore.Application.UseCases.SessionCase.Interfaces;
using AuthCore.Domain.Aggregates.SessionAggregate.Interfaces;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;

namespace AuthCore.Application.UseCases.SessionCase
{
    public sealed class RevokeSessions : IRevokeSession
    {
        private readonly ISessionState _sessionState;
        private readonly ISessionRepository _sessionRepository;

        public RevokeSessions(
            ISessionState sessionState,
            ISessionRepository sessionRepository)
        {
            _sessionState = sessionState;
            _sessionRepository = sessionRepository;
        }

        public async Task OnExecute()
        {
            var otherSessions = await _sessionState.GetOtherSessions();
            var deleteTasks = otherSessions.Select(s => _sessionRepository.Delete(s.Id));
            await Task.WhenAll(deleteTasks);
        }
    }
}
