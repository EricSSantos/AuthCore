namespace AuthCore.Domain.Commons.Interfaces.Persistence
{
    public interface IRedisContext
    {
        /// <summary>
        /// Armazena um objeto no Redis com tempo de expiração.
        /// </summary>
        Task Set<T>(string key, T value, TimeSpan ttl);

        /// <summary>
        /// Lê um objeto armazenado no Redis.
        /// </summary>
        Task<T?> Get<T>(string key);

        /// <summary>
        /// Remove uma chave do Redis.
        /// </summary>
        Task Delete(string key);

        /// <summary>
        /// Adiciona um valor em um índice do tipo Set.
        /// </summary>
        Task AddIndex(string key, string value, TimeSpan ttl);

        /// <summary>
        /// Retorna todos os valores de um índice.
        /// </summary>
        Task<IEnumerable<string>> GetIndexMembers(string key);

        /// <summary>
        /// Remove um valor de um índice.
        /// </summary>
        Task RemoveIndex(string key, string value);
    }
}
