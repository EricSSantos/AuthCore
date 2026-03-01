using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Base;

namespace AuthCore.Domain.Aggregates.ConfirmCodes
{
    /// <summary>Representa um código de confirmação temporário.</summary>
    public sealed class ConfirmCode : IAggregateRoot
    {
        public Guid Id { get; private set; }
        public int Code { get; private set; }
        public CodeType Type { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public int Attempts { get; private set; }

        #region Constructors

        /// <summary>Operação para criar instância de código.</summary>
        /// <param name="code">Valor numérico do código.</param>
        /// <param name="type">Tipo do código.</param>
        /// <param name="createdAt">Data de criação do código.</param>
        /// <param name="expiresAt">Data de expiração do código.</param>
        /// <param name="attempts">Quantidade de tentativas já realizadas.</param>
        private ConfirmCode(
            int code,
            CodeType type,
            DateTime createdAt,
            DateTime expiresAt,
            int attempts)
        {
            Id = Guid.NewGuid();
            Code = code;
            Type = type;
            CreatedAt = createdAt;
            ExpiresAt = expiresAt;
            Attempts = attempts;
            Validate();
        }

        /// <summary>Operação para criar instância de código.</summary>
        /// <param name="id">Identificador do código.</param>
        /// <param name="code">Valor numérico do código.</param>
        /// <param name="type">Tipo do código.</param>
        /// <param name="createdAt">Data de criação do código.</param>
        /// <param name="expiresAt">Data de expiração do código.</param>
        /// <param name="attempts">Quantidade de tentativas já realizadas.</param>
        private ConfirmCode(
            Guid id,
            int code,
            CodeType type,
            DateTime createdAt,
            DateTime expiresAt,
            int attempts)
        {
            Id = id;
            Code = code;
            Type = type;
            CreatedAt = createdAt;
            ExpiresAt = expiresAt;
            Attempts = attempts;
            Validate();
        }

        #endregion

        #region Factory

        /// <summary>Operação para criar código de confirmação.</summary>
        /// <param name="code">Valor numérico do código.</param>
        /// <param name="type">Tipo do código.</param>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        /// <param name="expiresAt">Data de expiração do código.</param>
        public static ConfirmCode Create(
            int code,
            CodeType type,
            DateTime utcNow,
            DateTime expiresAt)
        {
            return new ConfirmCode(code, type, utcNow, expiresAt, attempts: 0);
        }

        /// <summary>Operação para restaurar código de confirmação.</summary>
        /// <param name="id">Identificador do código.</param>
        /// <param name="code">Valor numérico do código.</param>
        /// <param name="type">Tipo do código.</param>
        /// <param name="createdAt">Data de criação do código.</param>
        /// <param name="expiresAt">Data de expiração do código.</param>
        /// <param name="attempts">Quantidade de tentativas já realizadas.</param>
        public static ConfirmCode Restore(
            Guid id,
            int code,
            CodeType type,
            DateTime createdAt,
            DateTime expiresAt,
            int attempts)
        {
            return new ConfirmCode(id, code, type, createdAt, expiresAt, attempts);
        }

        #endregion

        /// <summary>Operação para validar código informado.</summary>
        /// <param name="code">Código a validar.</param>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        /// <param name="maxAttempts">Quantidade máxima de tentativas permitidas.</param>
        public void Matching(int code, DateTime utcNow, int maxAttempts)
        {
            if (IsExpired(utcNow))
                throw new InvalidOrExpiredCodeException();

            if (maxAttempts < 1)
                throw new BadRequestException("O limite de tentativas do código é inválido.");

            Attempts += 1;

            if (Attempts > maxAttempts)
                throw new InvalidOrExpiredCodeException();

            if (Code != code)
                throw new InvalidOrExpiredCodeException();
        }

        #region Validation

        /// <summary>Operação para verificar expiração do código.</summary>
        /// <param name="utcNow">Data e hora atuais em UTC.</param>
        public bool IsExpired(DateTime utcNow)
        {
            return utcNow >= ExpiresAt;
        }

        /// <summary>Operação para validar código.</summary>
        private void Validate()
        {
            DigitCode.Validate(Code, "O código de verificação deve conter 6 dígitos.");

            if (Attempts < 0)
                throw new BadRequestException("O total de tentativas é inválido.");

            if (!Enum.IsDefined(typeof(CodeType), Type))
                throw new BadRequestException("O tipo de código informado é inválido.");

            if (CreatedAt == default)
                throw new BadRequestException("A data de criação do código é obrigatória.");

            if (ExpiresAt == default || ExpiresAt <= CreatedAt)
                throw new BadRequestException("A data de expiração do código é inválida.");
        }

        #endregion
    }
}
