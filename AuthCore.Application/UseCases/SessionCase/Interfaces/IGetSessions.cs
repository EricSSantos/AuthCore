using AuthCore.Application.Models.Responses;

namespace AuthCore.Application.UseCases.SessionCase.Interfaces
{
    /// <summary>
    /// Define operação para listar sessões do usuário.
    /// </summary>
    public interface IGetSessions
    {
        /// <summary>
        /// Obtém todas as sessões ativas do usuário.
        /// </summary>
        /// <returns>Coleção de sessões.</returns>
        Task<IEnumerable<SessionResponse>> OnExecuteAsync();
    }
}
