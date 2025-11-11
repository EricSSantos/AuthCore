namespace AuthCore.Domain.Aggregates.SessionAggregate.Interfaces
{
    /// <summary>
    /// Define operações de persistência do agregado Session.
    /// </summary>
    public interface ISessionRepository
    {
        /// <summary>
        /// Obtém uma sessão pelo identificador.
        /// </summary>
        Task<Session?> Get(string id);

        /// <summary>
        /// Obtém todas as sessões de um usuário.
        /// </summary>
        Task<IEnumerable<Session>> GetAllByUserId(Guid userId);

        /// <summary>
        /// Armazena ou atualiza uma sessão.
        /// </summary>
        Task Set(Session session);

        /// <summary>
        /// Remove uma sessão pelo identificador.
        /// </summary>
        Task Delete(string id);
    }
}
