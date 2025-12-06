using AuthCore.Domain.Aggregates.ConfirmCodeAggregate;

namespace AuthCore.Domain.Aggregates.MessagingAggregate.Payloads
{
    /// <summary>
    /// Representa os dados do e-mail de verificação de conta.
    /// </summary>
    public sealed class ConfirmEmailPayload : Payload
    {
        public int Code { get; init; }

        #region Constructors

        /// <summary>
        /// Inicializa o payload gerando um código seguro.
        /// </summary>
        public ConfirmEmailPayload()
        {
            Code = ConfirmCodeGenerator.Generate();
        }

        #endregion
    }
}
