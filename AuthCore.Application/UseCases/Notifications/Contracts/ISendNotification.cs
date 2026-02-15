using AuthCore.Application.Models.Requests;

namespace AuthCore.Application.UseCases.Notifications.Contracts
{
    /// <summary>Define operação para envio de e-mails.</summary>
    public interface ISendNotification
    {
        /// <summary>Operação para enviar o e-mail informado.</summary>
        /// <param name="request">Dados do e-mail.</param>
        Task OnExecuteAsync(SendNotificationRequest request);
    }
}
