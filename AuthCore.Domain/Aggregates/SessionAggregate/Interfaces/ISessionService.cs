using AuthCore.Domain.Aggregates.SessionAggregate;

namespace AuthCore.Domain.Aggregates.SessionAggregate.Interfaces
{
    /// <summary>
    /// Define operações de domínio relacionadas ao ciclo de vida de sessões.
    /// </summary>
    public interface ISessionService
    {
        /// <summary>
        /// Cria uma nova sessão respeitando regras globais do domínio.
        /// </summary>
        Task<SessionCreationResult> CreateSessionAsync(Guid userId, DeviceInfo deviceInfo, TimeSpan ttl, TimeSpan maxLifetime);

        /// <summary>
        /// Invalida uma sessão existente.
        /// </summary>
        Task InvalidateSessionAsync(string sessionId);
    }
}
