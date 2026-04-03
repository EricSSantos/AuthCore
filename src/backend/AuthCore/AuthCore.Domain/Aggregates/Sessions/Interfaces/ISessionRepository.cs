namespace AuthCore.Domain.Aggregates.Sessions.Interfaces
{
    /// <summary>Define operações de persistência do agregado Session.</summary>
    public interface ISessionRepository
    {
        /// <summary>Operação para obter sessão por identificador.</summary>
        /// <param name="id">Identificador da sessão.</param>
        Task<Session?> GetAsync(string id);

        /// <summary>Operação para obter sessões do usuário.</summary>
        /// <param name="userId">Identificador do usuário.</param>
        Task<IEnumerable<Session>> GetAllByUserIdAsync(Guid userId);

        /// <summary>Operação para armazenar sessão.</summary>
        /// <param name="session">Instância da sessão.</param>
        Task SetAsync(Session session);

        /// <summary>Operação para remover sessão.</summary>
        /// <param name="id">Identificador da sessão.</param>
        Task DeleteAsync(string id);

        /// <summary>Operação para remover sessões expiradas e revogadas antigas.</summary>
        /// <param name="utcNow">Data/hora atual em UTC.</param>
        /// <param name="revokedRetentionDays">Dias de retenção após revogação.</param>
        Task DeleteExpiredAsync(DateTime utcNow, int revokedRetentionDays);
    }
}
