namespace AuthCore.Domain.Core.Interfaces.Infrastructure.Web
{
    /// <summary>Define abstração para manipulação segura de cookies HTTP.</summary>
    /// <remarks>
    /// Fornece métodos para leitura, gravação e remoção de cookies.
    /// Implementações devem garantir configurações seguras como <c>HttpOnly</c>,
    /// <c>Secure</c> e <c>SameSite</c>.
    /// </remarks>
    public interface ICookie
    {
        /// <summary>Operação para obter valor de cookie.</summary>
        /// <param name="key">Nome do cookie.</param>
        /// <exception cref="UnauthorizedAccessException">
        /// Lançada quando o cookie não existe ou está vazio.
        /// </exception>
        string Get(string key);

        /// <summary>Operação para definir cookie.</summary>
        /// <param name="key">Nome do cookie.</param>
        /// <param name="value">Valor do cookie.</param>
        /// <param name="ttl">Tempo de vida do cookie.</param>
        void Set(string key, string value, TimeSpan ttl);

        /// <summary>Operação para definir cookie com controle de HttpOnly.</summary>
        /// <param name="key">Nome do cookie.</param>
        /// <param name="value">Valor do cookie.</param>
        /// <param name="ttl">Tempo de vida do cookie.</param>
        /// <param name="httpOnly">Define se o cookie é HttpOnly.</param>
        void Set(string key, string value, TimeSpan ttl, bool httpOnly);

        /// <summary>Operação para remover cookie.</summary>
        /// <param name="key">Nome do cookie.</param>
        void Remove(string key);
    }
}
