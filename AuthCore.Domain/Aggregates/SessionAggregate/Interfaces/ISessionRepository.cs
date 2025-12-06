namespace AuthCore.Domain.Aggregates.SessionAggregate.Interfaces
{
    /// <summary>
    /// Define operações de persistência do agregado Session.
    /// </summary>
    public interface ISessionRepository
    {
        /// <summary>
        /// Obtém a sessão pelo identificador.
        /// </summary>
        /// <param name="id">Identificador da sessão.</param>
        /// <returns>Sessão encontrada ou null.</returns>
        Task<Session?> GetAsync(string id);

        /// <summary>
        /// Obtém todas as sessões do usuário.
        /// </summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <returns>Sessões associadas ao usuário.</returns>
        Task<IEnumerable<Session>> GetAllByUserIdAsync(Guid userId);

        /// <summary>
        /// Armazena ou atualiza a sessão.
        /// </summary>
        /// <param name="session">Instância da sessão.</param>
        Task SetAsync(Session session);

        /// <summary>
        /// Remove a sessão pelo identificador.
        /// </summary>
        /// <param name="id">Identificador da sessão.</param>
        Task DeleteAsync(string id);
    }
}
