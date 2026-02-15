using AuthCore.Domain.Aggregates.Sessions;
using AuthCore.Domain.Aggregates.Sessions.Results;

namespace AuthCore.Domain.Aggregates.Sessions.Interfaces
{
    /// <summary>Define operações de domínio relacionadas ao ciclo de vida de sessões.</summary>
    public interface ISessionService
    {
        /// <summary>Operação para criar sessão.</summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="deviceInfo">Dados do dispositivo.</param>
        /// <param name="ttl">Tempo de expiração da sessão.</param>
        /// <param name="maxLifetime">Tempo máximo de vida útil.</param>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        Task<SessionCreationResult> CreateSessionAsync(Guid userId, DeviceInfo deviceInfo, TimeSpan ttl, TimeSpan maxLifetime, DateTime utcNow);

        /// <summary>Operação para invalidar sessão.</summary>
        /// <param name="sessionId">Identificador da sessão.</param>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        Task InvalidateSessionAsync(string sessionId, DateTime utcNow);
    }
}
