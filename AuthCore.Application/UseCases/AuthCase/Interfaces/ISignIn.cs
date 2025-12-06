using AuthCore.Application.Models.Requests;

namespace AuthCore.Application.UseCases.AuthCase.Interfaces
{
    /// <summary>
    /// Define operação para autenticar o usuário.
    /// </summary>
    public interface ISignIn
    {
        /// <summary>
        /// Realiza o processo de autenticação.
        /// </summary>
        /// <param name="request">Credenciais do usuário.</param>
        Task OnExecuteAsync(SignInRequest request);
    }
}
