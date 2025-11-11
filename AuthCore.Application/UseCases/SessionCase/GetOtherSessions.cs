using AuthCore.Application.Models.Responses;
using AuthCore.Application.UseCases.SessionCase.Interfaces;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;

namespace AuthCore.Application.UseCases.SessionCase
{
    public sealed class GetOtherSessions : IGetSessions
    {
        private readonly ISessionState _sessionState;

        public GetOtherSessions(ISessionState sessionState)
        {
            _sessionState = sessionState;
        }

        public async Task<IEnumerable<SessionResponse>> OnExecute()
        {
            var sessions = await _sessionState.GetOtherSessions();
            return sessions.Select(s => new SessionResponse
            {
                Id = s.Id,
                IpAddress = s.DeviceInfo.Ip,
                Platform = s.DeviceInfo.Platform,
                Browser = s.DeviceInfo.Browser,
                CreatedAt = s.CreatedAt
            });
        }
    }
}
