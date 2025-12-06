using AuthCore.Domain.Aggregates.ConfirmCodeAggregate;

namespace AuthCore.Domain.Aggregates.MessagingAggregate.Payloads
{
    /// <summary>
    /// Representa os dados do e-mail de recuperação de senha.
    /// </summary>
    public sealed class ForgotPasswordPayload : Payload
    {
        public int Code { get; init; }

        #region Constructors

        /// <summary>
        /// Inicializa o payload gerando um código seguro.
        /// </summary>
        public ForgotPasswordPayload()
        {
            Code = ConfirmCodeGenerator.Generate();
        }

        #endregion
    }
}
