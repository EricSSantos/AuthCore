namespace AuthCore.Domain.Aggregates.EmailAggregate
{
    /// <summary>
    /// Define o contrato para envio de e-mails transacionais.
    /// </summary>
    public interface IEmailPublisher
    {
        /// <summary>
        /// Envia o e-mail informado ao destinatário.
        /// </summary>
        /// <param name="email">Instância do e-mail a ser enviada.</param>
        Task Send(Email email);
    }
}
