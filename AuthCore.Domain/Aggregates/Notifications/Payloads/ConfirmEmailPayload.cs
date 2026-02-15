using AuthCore.Domain.Core.Exceptions;

namespace AuthCore.Domain.Aggregates.Notifications.Payloads
{
    /// <summary>Representa dados do e-mail de verificação de conta.</summary>
    public sealed class ConfirmEmailPayload : Payload
    {
        #region Constants

        private const int CODE_MIN = 100000;
        private const int CODE_MAX = 999999;

        #endregion

        public int Code { get; init; }

        #region Constructors

        /// <summary>Operação para inicializar payload.</summary>
        /// <param name="code">Código de confirmação.</param>
        public ConfirmEmailPayload(int code)
        {
            Code = code;
            Validate();
        }

        #endregion

        #region Validation

        /// <summary>Operação para validar payload.</summary>
        private void Validate()
        {
            if (Code < CODE_MIN || Code > CODE_MAX)
                throw new InvalidCodeFormatException("O código de confirmação deve conter 6 dígitos.");
        }

        #endregion
    }
}
