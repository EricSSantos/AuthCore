using AuthCore.Application.UseCases.Sessions.Interfaces;
using AuthCore.Domain.Aggregates.Sessions.Interfaces;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;

namespace AuthCore.Application.UseCases.Sessions
{
    /// <summary>Representa caso de uso de revogação de sessões.</summary>
    public sealed class RevokeSessions : IRevokeSession
    {
        private readonly ISessionState _sessionState;
        private readonly ISessionRepository _sessionRepository;

        /// <summary>Operação para criar instância de caso de uso.</summary>
        /// <param name="sessionState">Gerenciador de estado da sessão atual.</param>
        /// <param name="sessionRepository">Repositório de sessões.</param>
        public RevokeSessions(
            ISessionState sessionState,
            ISessionRepository sessionRepository)
        {
            _sessionState = sessionState;
            _sessionRepository = sessionRepository;
        }

        /// <summary>Operação para revogar a sessão do usuário.</summary>
        public async Task OnExecuteAsync()
        {
            var current = await _sessionState.GetCurrentSession();
            var sessions = await _sessionState.GetActiveSessions();

            var otherSessions = sessions.Where(s => s.Id.Value != current.Id.Value);
            var deleteTasks = otherSessions.Select(s => _sessionRepository.DeleteAsync(s.Id.Value));
            await Task.WhenAll(deleteTasks);
        }
    }
}
