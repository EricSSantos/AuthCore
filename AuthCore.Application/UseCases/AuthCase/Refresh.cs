using AuthCore.Application.UseCases.AuthCase.Interfaces;
using AuthCore.Domain.Aggregates.SessionAggregate.Interfaces;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;
using AuthCore.Domain.Core.Settings;

namespace AuthCore.Application.UseCases.AuthCase
{
    public sealed class Refresh : IRefresh
    {
        private readonly ISessionRepository _sessionRepository;
        private readonly IJwtTokenProvider _jwtTokenProvider;
        private readonly ISessionState _sessionState;
        private readonly SecuritySettings _settings;

        public Refresh(
            ISessionRepository sessionRepository,
            IJwtTokenProvider jwtTokenProvider,
            ISessionState sessionState,
            SecuritySettings settings)
        {
            _sessionRepository = sessionRepository;
            _jwtTokenProvider = jwtTokenProvider;
            _sessionState = sessionState;
            _settings = settings;
        }

        public async Task OnExecute()
        {
            var user = await _sessionState.GetCurrentUser();
            var session = await _sessionState.GetCurrentSession();

            session.Refresh(TimeSpan.FromDays(_settings.Session.ExpiresInDays));

            await _sessionRepository.Set(session);

            var newAccessToken = _jwtTokenProvider.Generate(user.Id);
            _sessionState.SetCookies(_sessionState.Session, newAccessToken);
        }
    }
}
