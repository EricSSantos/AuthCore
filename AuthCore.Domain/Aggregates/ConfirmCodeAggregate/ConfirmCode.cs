using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Base;

namespace AuthCore.Domain.Aggregates.ConfirmCodeAggregate
{
    /// <summary>
    /// Representa um código de confirmação temporário usado para validações.
    /// </summary>
    public sealed class ConfirmCode : IAggregateRoot
    {
        #region Properties

        public Guid Id { get; private set; }
        public int Code { get; private set; }
        public CodeType Type { get; private set; }
        public DateTime CreatedAt { get; private set; }

        #endregion

        #region Constructors

        private ConfirmCode(
            int code,
            CodeType type,
            DateTime createdAt)
        {
            Id = Guid.NewGuid();
            Code = code;
            Type = type;
            CreatedAt = createdAt;
            Validate();
        }

        private ConfirmCode(
            Guid id,
            int code,
            CodeType type,
            DateTime createdAt)
        {
            Id = id;
            Code = code;
            Type = type;
            CreatedAt = createdAt;
            Validate();
        }

        #endregion

        #region Factory

        /// <summary>
        /// Cria uma nova instância de código de confirmação.
        /// </summary>
        public static ConfirmCode Create(
            int code,
            CodeType type)
        {
            return new ConfirmCode(code, type, DateTime.UtcNow);
        }

        /// <summary>
        /// Restaura uma instância existente de código de confirmação (ex: a partir de cache ou persistência).
        /// </summary>
        public static ConfirmCode Restore(
            Guid id,
            int code,
            CodeType type,
            DateTime createdAt)
        {
            return new ConfirmCode(id, code, type, createdAt);
        }

        #endregion

        #region Behavior

        /// <summary>
        /// Verifica se o código informado corresponde ao código atual.
        /// </summary>
        public void Matching(int code)
        {
            if (Code != code)
                throw new BadRequestException("Código inválido ou expirado.");
        }

        #endregion

        #region Validation

        private void Validate()
        {
            if (Code.ToString().Length != 6)
                throw new BadRequestException("O código de verificação deve conter 6 dígitos.");
            if (!Enum.IsDefined(typeof(CodeType), Type))
                throw new BadRequestException("O tipo de código informado é inválido.");
        }

        #endregion
    }
}
