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
        Task Set(Guid userId, ConfirmCode verificationCode);

        /// <summary>
        /// Obtém o código de confirmação do usuário.
        /// </summary>
        Task<ConfirmCode?> Get(Guid userId, CodeType type);

        /// <summary>
        /// Remove o código de confirmação do usuário.
        /// </summary>
        Task Delete(Guid userId, CodeType type);
    }
}
