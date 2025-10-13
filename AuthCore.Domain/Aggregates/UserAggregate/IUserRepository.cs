using AuthCore.Domain.Commons.Interfaces.Repositories;

namespace AuthCore.Domain.Aggregates.UserAggregate
{
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// Obtém um usuário pelo endereço de e-mail.
        /// </summary>
        Task<User?> GetByEmail(string email);
    }
}
