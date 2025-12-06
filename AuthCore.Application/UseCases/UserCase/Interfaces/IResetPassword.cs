using AuthCore.Application.Models.Requests;

namespace AuthCore.Application.UseCases.UserCase.Interfaces
{
    /// <summary>
    /// Define operação para redefinir a senha do usuário.
    /// </summary>
    public interface IResetPassword
    {
        /// <summary>
        /// Redefine a senha com os dados informados.
        /// </summary>
        /// <param name="request">Dados para redefinição.</param>
        Task OnExecuteAsync(ResetPasswordRequest request);
    }
}
