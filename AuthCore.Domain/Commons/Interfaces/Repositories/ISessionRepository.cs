using AuthCore.Domain.Entities;

namespace AuthCore.Domain.Commons.Interfaces.Repositories
{
    public interface ISessionRepository
    {
        Task<Session?> Get(string key);
        Task Set(Session session);
        Task Delete(string key);
    }
}
