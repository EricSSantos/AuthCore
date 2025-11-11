using AuthCore.Domain.Core.Interfaces.Infrastructure.Persistence;

namespace AuthCore.Domain.Aggregates.UserAggregate.Interfaces
{
    /// <summary>
    /// Define operações de persistência do agregado User.
    /// </summary>
    public interface IUserRepository : IBaseRepository<User>
    {
        /// <summary>
        /// Obtém um usuário pelo e-mail informado.
        /// </summary>
        Task<User?> GetByEmail(string email);
    }
}
