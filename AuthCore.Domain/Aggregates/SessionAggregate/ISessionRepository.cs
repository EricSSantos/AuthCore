namespace AuthCore.Domain.Aggregates.SessionAggregate
{
    public interface ISessionRepository
    {
        /// <summary>
        /// Obtém uma sessão a partir do hash da sessão.
        /// </summary>
        Task<Session?> Get(string hashedSession);

        /// <summary>
        /// Obtém uma sessão a partir do identificador único.
        /// </summary>
        Task<Session?> GetById(Guid id);

        /// <summary>
        /// Retorna todas as sessões associadas a um usuário.
        /// </summary>
        Task<IEnumerable<Session>> GetByUserId(Guid userId);

        /// <summary>
        /// Persiste uma nova sessão no armazenamento Redis.
        /// </summary>
        Task Set(Session session);

        /// <summary>
        /// Exclui uma sessão a partir do hash da sessão.
        /// </summary>
        Task Delete(string hashedSession);

        /// <summary>
        /// Exclui uma sessão a partir do identificador único.
        /// </summary>
        Task DeleteById(Guid id);
    }
}
