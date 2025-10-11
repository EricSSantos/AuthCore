namespace AuthCore.Domain.Aggregates.SessionAggregate
{
    public interface ISessionRepository
    {
        Task<Session?> Get(string key);
        Task Set(Session session);
        Task Delete(string key);
    }
}
