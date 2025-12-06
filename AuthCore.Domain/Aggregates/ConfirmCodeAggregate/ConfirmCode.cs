using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Base;

namespace AuthCore.Domain.Aggregates.ConfirmCodeAggregate
{
    /// <summary>
    /// Representa um código de confirmação temporário usado para validações.
    /// </summary>
    public sealed class ConfirmCode : IAggregateRoot
    {
        public Guid Id { get; private set; }
        public int Code { get; private set; }
        public CodeType Type { get; private set; }
        public DateTime CreatedAt { get; private set; }

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
        /// Cria um novo código de confirmação.
        /// </summary>
        /// <param name="code">Valor numérico do código.</param>
        /// <param name="type">Tipo do código.</param>
        /// <returns>Instância criada de <see cref="ConfirmCode"/>.</returns>
        public static ConfirmCode Create(
            int code,
            CodeType type)
        {
            return new ConfirmCode(code, type, DateTime.UtcNow);
        }

        /// <summary>
        /// Restaura um código de confirmação existente.
        /// </summary>
        /// <param name="id">Identificador da instância.</param>
        /// <param name="code">Valor numérico do código.</param>
        /// <param name="type">Tipo do código.</param>
        /// <param name="createdAt">Data de criação.</param>
        /// <returns>Instância restaurada de <see cref="ConfirmCode"/>.</returns>
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
        /// Verifica se o código informado corresponde ao atual.
        /// </summary>
        /// <param name="code">Código a validar.</param>
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
