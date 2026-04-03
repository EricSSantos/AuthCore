using AuthCore.Application.Models.Responses;

namespace AuthCore.Application.UseCases.Sessions.Interfaces
{
    /// <summary>Define operação para listar sessões do usuário.</summary>
    public interface IGetSessions
    {
        /// <summary>Operação para obter todas as sessões ativas do usuário.</summary>
        Task<IEnumerable<SessionResponse>> OnExecuteAsync();
    }
}
