using AuthCore.Application.Models.Requests;

namespace AuthCore.Application.UseCases.Users.Interfaces
{
    /// <summary>Define operação para alterar a senha do usuário.</summary>
    public interface IChangePassword
    {
        /// <summary>Operação para alterar a senha com os dados informados.</summary>
        /// <param name="request">Dados para alteração da senha.</param>
        Task OnExecuteAsync(ChangePasswordRequest request);
    }
}
