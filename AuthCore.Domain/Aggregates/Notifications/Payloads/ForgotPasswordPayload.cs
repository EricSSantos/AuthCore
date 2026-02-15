namespace AuthCore.Domain.Aggregates.Notifications.Payloads
{
    /// <summary>Representa dados do e-mail de recuperação de senha.</summary>
    public sealed class ForgotPasswordPayload : Payload
    {
        public int Code { get; init; }

        #region Constructors

        /// <summary>Operação para inicializar payload.</summary>
        /// <param name="code">Código de recuperação.</param>
        public ForgotPasswordPayload(int code)
        {
            Code = code;
        }

        #endregion
    }
}
