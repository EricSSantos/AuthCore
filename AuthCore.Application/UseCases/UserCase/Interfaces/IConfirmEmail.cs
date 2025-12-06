using AuthCore.Application.Models.Requests;

namespace AuthCore.Application.UseCases.UserCase.Interfaces
{
    /// <summary>
    /// Define operação para confirmar o e-mail do usuário.
    /// </summary>
    public interface IConfirmEmail
    {
        /// <summary>
        /// Confirma o e-mail utilizando o código informado.
        /// </summary>
        /// <param name="request">Dados para confirmação.</param>
        Task OnExecuteAsync(ConfirmEmailRequest request);
    }
}
