namespace AuthCore.Domain.Aggregates.MessagingAggregate.Interfaces
{
    /// <summary>
    /// Define operações para envio de notificações por e-mail.
    /// </summary>
    public interface IEmailPublisher
    {
        /// <summary>
        /// Envia a notificação de e-mail.
        /// </summary>
        /// <param name="email">Dados da notificação.</param>
        Task SendAsync(Messaging email);
    }
}
