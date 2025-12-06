using AuthCore.Application.Models.Responses;

namespace AuthCore.Application.UseCases.UserCase.Interfaces
{
    /// <summary>
    /// Define operação para obter o usuário autenticado.
    /// </summary>
    public interface IGetCurrentUser
    {
        /// <summary>
        /// Retorna os dados do usuário atual.
        /// </summary>
        /// <returns>Dados do usuário.</returns>
        Task<UserResponse> OnExecuteAsync();
    }
}
