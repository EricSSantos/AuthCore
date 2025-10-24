using AuthCore.Domain.Commons.Interfaces.Persistence;

namespace AuthCore.Domain.Aggregates.UserAggregate
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<User?> GetByEmail(string email);
    }
}
