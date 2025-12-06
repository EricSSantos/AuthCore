using AuthCore.Application.Models.Requests;

namespace AuthCore.Application.UseCases.EmailCase.Interface
{
    /// <summary>
    /// Define operação para envio de e-mails.
    /// </summary>
    public interface ISendEmail
    {
        /// <summary>
        /// Envia o e-mail informado.
        /// </summary>
        /// <param name="request">Dados do e-mail.</param>
        Task OnExecuteAsync(SendEmailRequest request);
    }
}
