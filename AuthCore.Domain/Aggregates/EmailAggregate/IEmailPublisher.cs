namespace AuthCore.Domain.Aggregates.EmailAggregate
{
    /// <summary>
    /// Define o contrato para publicação ou envio de e-mails transacionais.
    /// </summary>
    public interface IEmailPublisher
    {
        /// <summary>
        /// Envia um e-mail para o destinatário especificado.
        /// </summary>
        /// <param name="email">Instância do e-mail a ser enviado.</param>
        Task Send(Email email);
    }
}
