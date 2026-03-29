using AuthCore.Domain.Aggregates.Sessions;
using AuthCore.Domain.Aggregates.Users;

namespace AuthCore.Domain.Core.Interfaces.Infrastructure.Security
{
    /// <summary>Define operações para gerenciar autenticação e sessões ativas.</summary>
    public interface ISessionState
    {
        string Session { get; }

        /// <summary>Operação para obter usuário autenticado.</summary>
        Task<User> GetCurrentUser();

        /// <summary>Operação para obter sessão atual.</summary>
        Task<Session> GetCurrentSession();

        /// <summary>Operação para obter usuário e sessão atual via cookie.</summary>
        Task<(User User, Session Session)> GetCurrentUserFromSession();

        /// <summary>Operação para obter todas as sessões ativas.</summary>
        Task<IReadOnlyCollection<Session>> GetActiveSessions();

        /// <summary>Operação para definir cookies de autenticação.</summary>
        /// <param name="rawSession">Identificador da sessão.</param>
        /// <param name="accessToken">Token de acesso.</param>
        void SetCookies(string rawSession, string accessToken);

        /// <summary>Operação para remover cookies de autenticação.</summary>
        void ClearCookies();
    }
}
