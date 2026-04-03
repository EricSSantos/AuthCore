using AuthCore.Application.Models.Requests;

namespace AuthCore.Application.UseCases.Users.Interfaces
{
    /// <summary>Define operação para confirmar o e-mail do usuário.</summary>
    public interface IConfirmEmail
    {
        /// <summary>Operação para confirmar o e-mail utilizando o código informado.</summary>
        /// <param name="request">Dados para confirmação.</param>
        Task OnExecuteAsync(ConfirmEmailRequest request);
    }
}
