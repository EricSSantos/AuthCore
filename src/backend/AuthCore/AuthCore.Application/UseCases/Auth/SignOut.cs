using AuthCore.Application.UseCases.Auth.Interfaces;
using AuthCore.Domain.Aggregates.Sessions.Interfaces;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;
using Microsoft.Extensions.Logging;

namespace AuthCore.Application.UseCases.Auth
{
    /// <summary>Representa caso de uso de encerramento de sessão.</summary>
    public sealed class SignOut : ISignOut
    {
        private readonly ISessionService _sessionService;
        private readonly ISessionState _sessionState;
        private readonly ILogger<SignOut> _logger;

        /// <summary>Operação para criar instância de caso de uso.</summary>
        /// <param name="sessionService">Serviço de domínio de sessões.</param>
        /// <param name="sessionState">Gerenciador de estado da sessão atual.</param>
        /// <param name="logger">Serviço de logging da operação.</param>
        public SignOut(
            ISessionService sessionService,
            ISessionState sessionState,
            ILogger<SignOut> logger)
        {
            _sessionService = sessionService;
            _sessionState = sessionState;
            _logger = logger;
        }

        /// <summary>Operação para realizar o logout do usuário.</summary>
        public async Task OnExecuteAsync()
        {
            var session = await _sessionState.GetCurrentSession();
            await _sessionService.InvalidateSessionAsync(session.Id.Value, DateTime.UtcNow);
            _sessionState.ClearCookies();
            _logger.LogInformation("Logout realizado. SessionId={SessionId}.", session.Id);
        }
    }
}
