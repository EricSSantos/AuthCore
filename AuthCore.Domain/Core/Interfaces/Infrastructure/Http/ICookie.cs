namespace AuthCore.Domain.Core.Interfaces.Infrastructure.Http
{
    /// <summary>
    /// Define uma abstração para manipulação de cookies HTTP de forma segura.
    /// </summary>
    /// <remarks>
    /// Essa interface fornece métodos genéricos para leitura, gravação e remoção de cookies.
    /// Implementações concretas devem garantir configurações seguras como <c>HttpOnly</c>,
    /// <c>Secure</c> e <c>SameSite</c>.
    /// </remarks>
    public interface ICookie
    {
        /// <summary>
        /// Obtém o valor de um cookie específico.
        /// </summary>
        /// <param name="key">Nome do cookie.</param>
        /// <returns>Valor do cookie correspondente.</returns>
        /// <exception cref="UnauthorizedAccessException">
        /// Lançada quando o cookie não é encontrado ou está vazio.
        /// </exception>
        string Get(string key);

        /// <summary>
        /// Define ou substitui o valor de um cookie com tempo de expiração definido.
        /// </summary>
        /// <param name="key">Nome do cookie.</param>
        /// <param name="value">Valor a ser armazenado.</param>
        /// <param name="ttl">Tempo de vida (Time To Live) do cookie.</param>
        void Set(string key, string value, TimeSpan ttl);

        /// <summary>
        /// Remove um cookie existente do cliente.
        /// </summary>
        /// <param name="key">Nome do cookie a ser removido.</param>
        void Remove(string key);
    }
}
