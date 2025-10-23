using AuthCore.Domain.Commons.Exceptions;
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

        private ConfirmCode()
        { }

        private ConfirmCode(int code, CodeType type)
        {
            if (code.ToString().Length != 6)
                throw new DomainException("O código de verificação deve conter 6 dígitos.");

            Type = type;
            Code = code;
            CreatedAt = DateTime.UtcNow;
        }

        #endregion

        #region Factory

        public static ConfirmCode Create(int code, CodeType type)
        {
            return new ConfirmCode(code, type);
        }

        #endregion
    }
}
