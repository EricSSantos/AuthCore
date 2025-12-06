using AuthCore.Domain.Core.Interfaces.Infrastructure.Persistence;

namespace AuthCore.Domain.Aggregates.UserAggregate.Interfaces
{
    /// <summary>
    /// Define operações de persistência do agregado User.
    /// </summary>
    public interface IUserRepository : IBaseRepository<User>
    {
        /// <summary>
        /// Obtém o usuário pelo e-mail.
        /// </summary>
        /// <param name="email">E-mail do usuário.</param>
        /// <returns>Usuário encontrado ou null.</returns>
        Task<User?> GetByEmailAsync(string email);
    }
}
