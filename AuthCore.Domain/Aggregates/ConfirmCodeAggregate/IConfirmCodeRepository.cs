namespace AuthCore.Domain.Aggregates.ConfirmCodeAggregate
{
    /// <summary>
    /// Define o contrato para armazenamento e recuperação de códigos de confirmação temporários.
    /// </summary>
    public interface IConfirmCodeRepository
    {
        /// <summary>
        /// Armazena um código de confirmação temporário associado ao usuário.
        /// </summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="verificationCode">Código de confirmação a ser armazenado.</param>
        Task Set(Guid userId, ConfirmCode verificationCode);

        /// <summary>
        /// Obtém o código de confirmação atual do usuário, se existir.
        /// </summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="type">Tipo do código de confirmação.</param>
        /// <returns>Código de confirmação ou <c>null</c> se não encontrado.</returns>
        Task<ConfirmCode?> Get(Guid userId, CodeType type);

        /// <summary>
        /// Remove o código de confirmação do usuário.
        /// </summary>
        /// <param name="userId">Identificador do usuário.</param>
        /// <param name="type">Tipo do código de confirmação.</param>
        Task Delete(Guid userId, CodeType type);
    }
}
