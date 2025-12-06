using AuthCore.Application.Models.Requests;

namespace AuthCore.Application.UseCases.UserCase.Interfaces
{
    /// <summary>
    /// Define operação para adicionar um novo usuário.
    /// </summary>
    public interface IAddUser
    {
        /// <summary>
        /// Cria o usuário com os dados informados.
        /// </summary>
        /// <param name="request">Dados do novo usuário.</param>
        Task OnExecuteAsync(AddUserRequest request);
    }
}
