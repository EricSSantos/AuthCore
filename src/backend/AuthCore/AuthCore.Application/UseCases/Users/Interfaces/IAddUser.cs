using AuthCore.Application.Models.Requests;

namespace AuthCore.Application.UseCases.Users.Interfaces
{
    /// <summary>Define operação para adicionar um novo usuário.</summary>
    public interface IAddUser
    {
        /// <summary>Operação para criar o usuário com os dados informados.</summary>
        /// <param name="request">Dados do novo usuário.</param>
        Task OnExecuteAsync(AddUserRequest request);
    }
}
