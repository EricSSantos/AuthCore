using AuthCore.Application.UseCases.Auth.Contracts;
using AuthCore.Domain.Aggregates.Sessions.Contracts;
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

        public SignOut(
            ISessionService sessionService,
            ISessionState sessionState,
            ILogger<SignOut> logger)
        {
            _sessionService = sessionService;
            _sessionState = sessionState;
            _logger = logger;
        }

        public async Task OnExecuteAsync()
        {
            var session = await _sessionState.GetCurrentSession();
            await _sessionService.InvalidateSessionAsync(session.Id, DateTime.UtcNow);
            _sessionState.ClearCookies();
            _logger.LogInformation("Logout realizado. SessionId={SessionId}.", session.Id);
        }
    }
}
