using AuthCore.Domain.Aggregates.SessionAggregate;
using AuthCore.Domain.Aggregates.UserAggregate;

namespace AuthCore.Domain.Core.Interfaces.Infrastructure.Security
{
    /// <summary>
    /// Define métodos para gerenciar o estado de autenticação e sessões ativas do usuário.
    /// </summary>
    public interface ISessionState
    {
        /// <summary>
        /// Retorna o identificador da sessão atual.
        /// </summary>
        string Session { get; }

        /// <summary>
        /// Retorna o usuário autenticado associado à sessão atual.
        /// </summary>
        /// <returns>Usuário autenticado e ativo.</returns>
        Task<User> GetCurrentUser();

        /// <summary>
        /// Retorna a sessão atual do usuário autenticado.
        /// </summary>
        /// <returns>Instância da sessão ativa e validada.</returns>
        Task<Session> GetCurrentSession();

        /// <summary>
        /// Retorna todas as outras sessões ativas do mesmo usuário, exceto a atual.
        /// </summary>
        /// <returns>Coleção somente leitura de sessões ativas.</returns>
        Task<IReadOnlyCollection<Session>> GetOtherSessions();

        /// <summary>
        /// Cria ou atualiza os dados de autenticação do usuário.
        /// </summary>
        /// <param name="rawSession">Identificador da sessão.</param>
        /// <param name="accessToken">Token de acesso (JWT).</param>
        void SetCookies(string rawSession, string accessToken);

        /// <summary>
        /// Remove os dados de autenticação do usuário.
        /// </summary>
        void ClearCookies();
    }
}
