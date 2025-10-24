namespace AuthCore.Domain.Aggregates.EmailAggregate
{
    public interface IEmailService
    {
        Task Send(Email email);
    }
}
