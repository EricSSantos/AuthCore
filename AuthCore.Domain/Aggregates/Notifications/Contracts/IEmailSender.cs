namespace AuthCore.Domain.Aggregates.Notifications.Contracts
{
    /// <summary>Define operações para envio de notificações por e-mail.</summary>
    public interface IEmailSender
    {
        /// <summary>Operação para enviar notificação por e-mail.</summary>
        /// <param name="notification">Notificação a ser enviada.</param>
        Task SendAsync(Notification notification);
    }
}
