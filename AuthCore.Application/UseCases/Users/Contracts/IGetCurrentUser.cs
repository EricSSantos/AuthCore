using AuthCore.Application.Models.Responses;

namespace AuthCore.Application.UseCases.Users.Contracts
{
    /// <summary>Define operação para obter o usuário autenticado.</summary>
    public interface IGetCurrentUser
    {
        /// <summary>Operação para retornar os dados do usuário atual.</summary>
        Task<UserResponse> OnExecuteAsync();
    }
}
