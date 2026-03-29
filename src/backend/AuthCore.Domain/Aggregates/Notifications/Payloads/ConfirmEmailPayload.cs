namespace AuthCore.Domain.Aggregates.Notifications.Payloads
{
    /// <summary>Representa dados do e-mail de verificação de conta.</summary>
    public sealed class ConfirmEmailPayload : Payload
    {
        public override NotificationType Type
        {
            get { return NotificationType.ConfirmEmail; }
        }

        public int Code { get; init; }

        #region Constructors

        /// <summary>Operação para inicializar payload.</summary>
        /// <param name="code">Código de confirmação.</param>
        public ConfirmEmailPayload(int code)
        {
            Code = code;
        }

        #endregion
    }
}
