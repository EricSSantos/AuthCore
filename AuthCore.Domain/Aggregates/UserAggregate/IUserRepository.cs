using AuthCore.Domain.Core.Interfaces.Persistence;

namespace AuthCore.Domain.Aggregates.UserAggregate
{
    /// <summary>
    /// Define as operações de persistência específicas do agregado <see cref="User"/>.
    /// </summary>
    public interface IUserRepository : IBaseRepository<User>
    {
        /// <summary>
        /// Obtém um usuário pelo endereço de e-mail.
        /// Retorna null se nenhum registro for encontrado.
        /// </summary>
        Task<User?> GetByEmail(string email);
    }
}
