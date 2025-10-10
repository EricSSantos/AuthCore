using AuthCore.Domain.Entities;

namespace AuthCore.Domain.Commons.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmail(string email);
    }
}
