using AuthCore.Domain.Commons.Interfaces.Persistence;

namespace AuthCore.Domain.Aggregates.UserAggregate
{
    public interface IUserRepository : IBaseRepository<User>
    {
        /// <summary>
        /// Obtém um usuário pelo endereço de e-mail.
        /// </summary>
        Task<User?> GetByEmail(string email);
    }
}
