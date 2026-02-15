using System;

namespace AuthCore.Domain.Aggregates.Notifications.Payloads
{
    /// <summary>Representa dados do e-mail de recuperação de senha.</summary>
    public sealed class ForgotPasswordPayload : Payload
    {
        #region Constants

        private const int CODE_MIN = 100000;
        private const int CODE_MAX = 999999;

        #endregion

        public int Code { get; init; }

        #region Constructors

        /// <summary>Operação para inicializar payload.</summary>
        /// <param name="code">Código de recuperação.</param>
        public ForgotPasswordPayload(int code)
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
                throw new ArgumentOutOfRangeException(nameof(Code));
        }

        #endregion
    }
}
