namespace AuthCore.Domain.Aggregates.ConfirmCodeAggregate.Interfaces
{
    /// <summary>
    /// Define operações de persistência do agregado ConfirmCode.
    /// </summary>
    public interface IConfirmCodeRepository
    {
        /// <summary>
        /// Armazena o código de confirmação do usuário.
        /// </summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="verificationCode">Instância do código a armazenar.</param>
        Task SetAsync(Guid userId, ConfirmCode verificationCode);

        /// <summary>
        /// Obtém o código de confirmação do usuário.
        /// </summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="type">Tipo de código solicitado.</param>
        /// <returns>Código encontrado ou null.</returns>
        Task<ConfirmCode?> GetAsync(Guid userId, CodeType type);

        /// <summary>
        /// Remove o código de confirmação do usuário.
        /// </summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="type">Tipo de código a remover.</param>
        Task DeleteAsync(Guid userId, CodeType type);
    }
}
