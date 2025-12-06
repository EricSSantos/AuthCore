using AuthCore.Domain.Aggregates.SessionAggregate;
using AuthCore.Domain.Aggregates.UserAggregate;

namespace AuthCore.Domain.Core.Interfaces.Infrastructure.Security
{
    /// <summary>
    /// Define operações para gerenciar autenticação e sessões ativas.
    /// </summary>
    public interface ISessionState
    {
        /// <summary>
        /// Obtém o identificador da sessão atual.
        /// </summary>
        string Session { get; }

        /// <summary>
        /// Obtém o usuário autenticado da sessão atual.
        /// </summary>
        /// <returns>Usuário autenticado.</returns>
        Task<User> GetCurrentUser();

        /// <summary>
        /// Obtém a sessão ativa do usuário.
        /// </summary>
        /// <returns>Sessão atual validada.</returns>
        Task<Session> GetCurrentSession();

        /// <summary>
        /// Obtém as demais sessões ativas do usuário.
        /// </summary>
        /// <returns>Coleção somente leitura de sessões.</returns>
        Task<IReadOnlyCollection<Session>> GetOtherSessions();

        /// <summary>
        /// Define ou atualiza cookies de autenticação.
        /// </summary>
        /// <param name="rawSession">Identificador da sessão.</param>
        /// <param name="accessToken">Token de acesso.</param>
        void SetCookies(string rawSession, string accessToken);

        /// <summary>
        /// Remove cookies de autenticação.
        /// </summary>
        void ClearCookies();
    }
}
