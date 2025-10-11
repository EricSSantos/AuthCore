using AuthCore.Domain.Commons.Interfaces.Repositories;

namespace AuthCore.Domain.Aggregates.UserAggregate
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmail(string email);
    }
}
