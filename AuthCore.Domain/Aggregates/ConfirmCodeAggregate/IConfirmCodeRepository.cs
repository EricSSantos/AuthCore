namespace AuthCore.Domain.Aggregates.ConfirmCodeAggregate
{
    public interface IConfirmCodeRepository
    {
        Task Set(Guid userId, ConfirmCode verificationCode);
        Task<object?> Get(Guid userId, CodeType type);
    }
}
