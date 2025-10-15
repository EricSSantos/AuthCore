namespace AuthCore.Domain.Aggregates.EmailAggregate
{
    public interface IEmailService
    {
        /// <summary>
        /// Responsável por enfileirar e-mails no RabbitMQ.
        /// </summary>
        Task Send(Email email);
    }
}
