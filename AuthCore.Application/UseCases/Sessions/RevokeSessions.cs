using AuthCore.Application.UseCases.Sessions.Contracts;
using AuthCore.Domain.Aggregates.Sessions.Contracts;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;

namespace AuthCore.Application.UseCases.Sessions
{
    /// <summary>Representa caso de uso de revogação de sessões.</summary>
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

        public async Task OnExecuteAsync()
        {
            var current = await _sessionState.GetCurrentSession();
            var sessions = await _sessionState.GetActiveSessions();

            var otherSessions = sessions.Where(s => s.Id != current.Id);
            var deleteTasks = otherSessions.Select(s => _sessionRepository.DeleteAsync(s.Id));
            await Task.WhenAll(deleteTasks);
        }
    }
}
