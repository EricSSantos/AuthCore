using AuthCore.Domain.Entities;

namespace AuthCore.Domain.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmail(string email);
    }
}
