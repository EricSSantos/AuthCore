using AuthCore.Application.Models.Responses;
using AuthCore.Application.UseCases.Sessions.Interfaces;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;

namespace AuthCore.Application.UseCases.Sessions
{
    /// <summary>Representa caso de uso de listagem de sessões.</summary>
    public sealed class GetSessions : IGetSessions
    {
        private readonly ISessionState _sessionState;

        public GetSessions(ISessionState sessionState)
        {
            _sessionState = sessionState;
        }

        public async Task<IEnumerable<SessionResponse>> OnExecuteAsync()
        {
            var current = await _sessionState.GetCurrentSession();
            var sessions = await _sessionState.GetActiveSessions();
            return sessions.Select(s => new SessionResponse
            {
                Id = s.Id,
                IpAddress = s.DeviceInfo.Ip,
                Platform = s.DeviceInfo.Platform,
                Browser = s.DeviceInfo.Browser,
                CreatedAt = s.CreatedAt,
                IsCurrent = s.Id == current.Id
            });
        }
    }
}
