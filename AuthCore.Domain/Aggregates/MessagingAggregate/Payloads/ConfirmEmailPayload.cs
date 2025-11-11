using AuthCore.Domain.Aggregates.ConfirmCodeAggregate;

namespace AuthCore.Domain.Aggregates.MessagingAggregate.Payloads
{
    /// <summary>
    /// Representa os dados do e-mail de verificação de conta.
    /// </summary>
    public sealed class ConfirmEmailPayload : Payload
    {
        #region Properties

        /// <summary>
        /// Armazena o código de verificação de 6 dígitos.
        /// </summary>
        public int Code { get; init; }

        #endregion

        #region Constructors

        /// <summary>
        /// Inicializa o payload com um código gerado de forma segura.
        /// </summary>
        public ConfirmEmailPayload()
        {
            Code = ConfirmCodeGenerator.Generate();
        }

        #endregion
    }
}
