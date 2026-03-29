namespace AuthCore.Domain.Aggregates.Users.Interfaces
{
    /// <summary>Define operações de persistência do agregado User.</summary>
    public interface IUserRepository
    {
        /// <summary>Operação para obter usuário por identificador.</summary>
        /// <param name="id">Identificador do usuário.</param>
        Task<User?> GetByIdAsync(Guid id);

        /// <summary>Operação para obter usuário por e-mail.</summary>
        /// <param name="email">E-mail do usuário.</param>
        Task<User?> GetByEmailAsync(string email);

        /// <summary>Operação para verificar e-mail cadastrado.</summary>
        /// <param name="email">E-mail do usuário.</param>
        Task<bool> EmailExistsAsync(string email);

        /// <summary>Operação para adicionar usuário.</summary>
        /// <param name="user">Usuário a ser adicionado.</param>
        Task AddAsync(User user);

        /// <summary>Operação para atualizar usuário.</summary>
        /// <param name="user">Usuário a ser atualizado.</param>
        Task UpdateAsync(User user);

        /// <summary>Operação para remover usuário.</summary>
        /// <param name="user">Usuário a ser removido.</param>
        Task DeleteAsync(User user);
    }
}
