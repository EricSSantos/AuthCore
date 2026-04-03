namespace AuthCore.Domain.Aggregates.ConfirmCodes.Interfaces
{
    /// <summary>Define operações de persistência do agregado ConfirmCode.</summary>
    public interface IConfirmCodeRepository
    {
        /// <summary>Operação para armazenar código de confirmação.</summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="verificationCode">Código de confirmação.</param>
        Task SetAsync(Guid userId, ConfirmCode verificationCode);

        /// <summary>Operação para obter código de confirmação.</summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="type">Tipo do código.</param>
        Task<ConfirmCode?> GetAsync(Guid userId, CodeType type);

        /// <summary>Operação para remover código de confirmação.</summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="type">Tipo do código.</param>
        Task DeleteAsync(Guid userId, CodeType type);

        /// <summary>Operação para remover códigos de confirmação expirados.</summary>
        Task DeleteExpiredAsync();
    }
}
