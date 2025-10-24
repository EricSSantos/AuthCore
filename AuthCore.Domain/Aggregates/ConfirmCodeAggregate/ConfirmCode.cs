using AuthCore.Domain.Shared;

namespace AuthCore.Domain.Aggregates.ConfirmCodeAggregate
{
    public sealed class ConfirmCode : Entity
    {
        #region Properties

        public int Code { get; private set; }
        public CodeType Type { get; private set; }
        public DateTime CreatedAt { get; private set; }

        #endregion

        #region Constructors

        protected ConfirmCode() { }

        private ConfirmCode(
            int code,
            CodeType type,
            DateTime createdAt)
        {
            Code = code;
            Type = type;
            CreatedAt = createdAt;
            Validate();
        }

        #endregion

        #region Factory

        public static ConfirmCode Create(int code, CodeType type)
        {
            return new ConfirmCode(code, type, DateTime.UtcNow);
        }

        #endregion

        #region Behavior

        /// <summary>
        /// Restaura uma instância de código de confirmação existente.
        /// </summary>
        public static ConfirmCode FromPersistence(int code, CodeType type, DateTime createdAt)
        {
            return new ConfirmCode(code, type, createdAt);
        }

        /// <summary>
        /// Verifica se o código informado corresponde ao código atual.
        /// </summary>
        public bool IsMatching(int code)
        {
            return Code == code;
        }

        #endregion

        #region Private Methods

        private void Validate()
        {
            var validate = Validator();

            if (Code.ToString().Length != 6)
                validate.AddError("O código de verificação deve conter 6 dígitos.");
            if (!Enum.IsDefined(typeof(CodeType), Type))
                validate.AddError("O tipo de código informado é inválido.");

            validate.ThrowIfInvalid();
        }

        #endregion
    }
}
