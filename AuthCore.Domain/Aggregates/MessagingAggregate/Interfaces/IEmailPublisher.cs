namespace AuthCore.Domain.Aggregates.MessagingAggregate.Interfaces
{
    /// <summary>
    /// Define o contrato para enviar notificações por e-mail.
    /// </summary>
    public interface IEmailPublisher
    {
        /// <summary>
        /// Envia a notificação informada.
        /// </summary>
        /// <param name="email">Notificação a ser enviada.</param>
        Task Send(Messaging email);
    }
}
