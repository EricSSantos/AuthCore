using AuthCore.Application.UseCases.AuthCase.Interfaces;
using AuthCore.Domain.Aggregates.SessionAggregate.Interfaces;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;

namespace AuthCore.Application.UseCases.AuthCase
{
    public sealed class SignOut : ISignOut
    {
        private readonly ISessionService _sessionService;
        private readonly ISessionState _sessionState;

        public SignOut(
            ISessionService sessionService,
            ISessionState sessionState)
        {
            _sessionService = sessionService;
            _sessionState = sessionState;
        }

        public async Task OnExecuteAsync()
        {
            var session = await _sessionState.GetCurrentSession();
            await _sessionService.InvalidateSessionAsync(session.Id);
            _sessionState.ClearCookies();
        }
    }
}
