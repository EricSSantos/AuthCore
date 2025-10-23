using AuthCore.Domain.Commons.Exceptions;
using AuthCore.Domain.Shared;
using System.Text.RegularExpressions;

namespace AuthCore.Domain.Aggregates.UserAggregate
{
    public sealed class Password : ValueObject
    {
        #region Constants

        private const int MIN_LENGTH = 8;
        private const int MAX_LENGTH = 64;

        #endregion

        #region Properties

        public string Value { get; }

        #endregion

        #region Constructors

        protected Password() { }

        private Password(string hashedPassword)
        {
            Value = hashedPassword;
        }

        #endregion

        #region Factory

        public static Password Create(string hashedPassword)
        {
            return new Password(hashedPassword);
        }

        #endregion

        #region Behavior

        public static void EnsureIsValid(string plainPassword)
        {
            if (string.IsNullOrWhiteSpace(plainPassword))
                throw new DomainException("A senha não pode ser vazia.");
            if (plainPassword.Length < MIN_LENGTH)
                throw new DomainException($"A senha deve ter pelo menos {MIN_LENGTH} caracteres.");
            if (plainPassword.Length > MAX_LENGTH)
                throw new DomainException($"A senha não pode exceder {MAX_LENGTH} caracteres.");
            if (!IsStrong(plainPassword))
                throw new DomainException("A senha deve conter letras maiúsculas e minúsculas.");
        }

        private static bool IsStrong(string password)
        {
            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z]).{8,}$");
            return regex.IsMatch(password);
        }

        protected override IEnumerable<object?> GetValues()
        {
            yield return Value;
        }

        #endregion
    }
}
