namespace AuthCore.Domain.Aggregates.EmailAggregate
{
    public interface IEmailService
    {
        /// <summary>
        /// Envia uma mensagem de e-mail para a fila de processamento.
        /// </summary>
        /// <param name="to">Endereço de e-mail do destinatário.</param>
        /// <param name="fullName">Nome completo do destinatário.</param>
        /// <param name="type">Tipo do e-mail (ex: ResetPassword, Welcome, ConfirmEmail).</param>
        /// <param name="data">Dados adicionais para o template do e-mail.</param>
        Task Send(string to, string fullName, EmailType type, object? data = null);
    }
}
