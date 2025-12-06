namespace AuthCore.Domain.Aggregates.UserAggregate.Interfaces
{
    /// <summary>
    /// Define operações de persistência do agregado User.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Obtém um usuário pelo identificador.
        /// </summary>
        Task<User?> GetByIdAsync(Guid id);

        /// <summary>
        /// Obtém um usuário pelo e-mail informado.
        /// </summary>
        Task<User?> GetByEmailAsync(string email);

        /// <summary>
        /// Verifica se o e-mail já está cadastrado.
        /// </summary>
        Task<bool> EmailExistsAsync(string email);

        /// <summary>
        /// Adiciona um novo usuário ao repositório.
        /// </summary>
        Task AddAsync(User user);

        /// <summary>
        /// Atualiza um usuário existente.
        /// </summary>
        Task UpdateAsync(User user);

        /// <summary>
        /// Remove um usuário existente.
        /// </summary>
        Task DeleteAsync(User user);
    }
}
