namespace AuthCore.Domain.Core.Interfaces.Infrastructure.Http
{
    /// <summary>
    /// Define abstração para manipulação segura de cookies HTTP.
    /// </summary>
    /// <remarks>
    /// Fornece métodos para leitura, gravação e remoção de cookies.
    /// Implementações devem garantir configurações seguras como <c>HttpOnly</c>,
    /// <c>Secure</c> e <c>SameSite</c>.
    /// </remarks>
    public interface ICookie
    {
        /// <summary>
        /// Obtém o valor de um cookie.
        /// </summary>
        /// <param name="key">Nome do cookie.</param>
        /// <returns>Valor do cookie correspondente.</returns>
        /// <exception cref="UnauthorizedAccessException">
        /// Lançada quando o cookie não existe ou está vazio.
        /// </exception>
        string Get(string key);

        /// <summary>
        /// Define ou substitui um cookie com tempo de expiração.
        /// </summary>
        /// <param name="key">Nome do cookie.</param>
        /// <param name="value">Valor do cookie.</param>
        /// <param name="ttl">Tempo de vida do cookie.</param>
        void Set(string key, string value, TimeSpan ttl);

        /// <summary>
        /// Remove o cookie informado.
        /// </summary>
        /// <param name="key">Nome do cookie.</param>
        void Remove(string key);
    }
}
