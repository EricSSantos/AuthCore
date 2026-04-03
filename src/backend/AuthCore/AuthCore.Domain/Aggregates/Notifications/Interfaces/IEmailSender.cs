namespace AuthCore.Domain.Aggregates.Notifications.Interfaces
{
    /// <summary>Define operações para envio de notificações por e-mail.</summary>
    public interface IEmailSender
    {
        /// <summary>Operação para enviar mensagem por e-mail.</summary>
        /// <param name="message">Mensagem a ser enviada.</param>
        Task SendAsync(EmailMessage message);
    }
}
