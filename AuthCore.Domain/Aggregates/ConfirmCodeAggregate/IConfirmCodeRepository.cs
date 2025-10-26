namespace AuthCore.Domain.Aggregates.ConfirmCodeAggregate
{
    /// <summary>
    /// Define as operações de armazenamento e recuperação de códigos de confirmação temporários.
    /// </summary>
    public interface IConfirmCodeRepository
    {
        /// <summary>
        /// Armazena um código de confirmação temporário para um usuário.
        /// </summary>
        Task Set(Guid userId, ConfirmCode verificationCode);

        /// <summary>
        /// Recupera o código de confirmação atual do usuário, se existir.
        /// </summary>
        Task<ConfirmCode?> Get(Guid userId, CodeType type);
    }
}
