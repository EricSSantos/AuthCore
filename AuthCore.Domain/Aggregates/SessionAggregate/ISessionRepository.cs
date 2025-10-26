namespace AuthCore.Domain.Aggregates.SessionAggregate
{
    /// <summary>
    /// Define o contrato para operações de persistência e recuperação de sessões de usuário.
    /// </summary>
    public interface ISessionRepository
    {
        /// <summary>
        /// Obtém uma sessão específica pelo seu identificador.
        /// </summary>
        Task<Session?> Get(string id);

        /// <summary>
        /// Retorna todas as sessões associadas a um usuário.
        /// </summary>
        Task<IEnumerable<Session>> GetAllByUserId(Guid userId);

        /// <summary>
        /// Armazena ou atualiza uma sessão.
        /// </summary>
        Task Set(Session session);

        /// <summary>
        /// Remove uma sessão pelo seu identificador.
        /// </summary>
        Task Delete(string id);
    }
}
