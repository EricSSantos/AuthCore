using AuthCore.Application.Models.Responses;
using AuthCore.Application.UseCases.Sessions.Interfaces;
using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;

namespace AuthCore.Application.UseCases.Sessions
{
    /// <summary>Representa caso de uso de listagem de sessões.</summary>
    public sealed class GetSessions : IGetSessions
    {
        private readonly ISessionState _sessionState;

        /// <summary>Operação para criar instância de caso de uso.</summary>
        /// <param name="sessionState">Gerenciador de estado da sessão atual.</param>
        public GetSessions(ISessionState sessionState)
        {
            _sessionState = sessionState;
        }

        /// <summary>Operação para obter todas as sessões ativas do usuário.</summary>
        public async Task<IEnumerable<SessionResponse>> OnExecuteAsync()
        {
            var current = await _sessionState.GetCurrentSession();
            var sessions = await _sessionState.GetActiveSessions();
            return sessions.Select(s => new SessionResponse
            {
                Id = s.Id.Value,
                IpAddress = s.DeviceInfo.Ip,
                Platform = s.DeviceInfo.Platform,
                Browser = s.DeviceInfo.Browser,
                CreatedAt = s.CreatedAt,
                IsCurrent = s.Id.Value == current.Id.Value
            });
        }
    }
}
