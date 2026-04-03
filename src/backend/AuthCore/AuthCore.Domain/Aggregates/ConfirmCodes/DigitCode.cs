using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Base;

namespace AuthCore.Domain.Aggregates.ConfirmCodes
{
    /// <summary>Representa um código numérico de 6 dígitos.</summary>
    public sealed class DigitCode : IValueObject
    {
        public const int MIN_VALUE = 100_000;
        public const int MAX_VALUE = 999_999;
        public int Value { get; }

        #region Constructors

        /// <summary>Operação para criar instância de código.</summary>
        /// <param name="value">Valor numérico do código.</param>
        private DigitCode(int value)
        {
            Value = value;
        }

        #endregion

        #region Factory

        /// <summary>Operação para criar código.</summary>
        /// <param name="value">Valor numérico do código.</param>
        public static DigitCode Create(int value)
        {
            Validate(value);
            return new DigitCode(value);
        }

        #endregion

        #region Validation

        /// <summary>Operação para verificar se código é válido.</summary>
        /// <param name="value">Valor numérico do código.</param>
        public static bool IsValid(int value)
        {
            return value is >= MIN_VALUE and <= MAX_VALUE;
        }

        /// <summary>Operação para validar código.</summary>
        /// <param name="value">Valor numérico do código.</param>
        /// <param name="errorMessage">Mensagem de erro opcional.</param>
        public static void Validate(int value, string? errorMessage = null)
        {
            if (!IsValid(value))
                throw new InvalidCodeFormatException(errorMessage ?? "O código deve conter 6 dígitos.");
        }

        #endregion
    }
}
