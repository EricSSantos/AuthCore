using AuthCore.Domain.Shared;
using System.Text.RegularExpressions;

namespace AuthCore.Domain.Aggregates.UserAggregate
{
    public sealed class Password : ValueObject
    {
        private const int MIN_LENGTH = 8;
        private const int MAX_LENGTH = 24;

        public string Value { get; }

        protected Password() { }

        private Password(string hashedPassword)
        {
            Value = hashedPassword;
        }

        public static Password Create(string hashedPassword)
        {
            return new Password(hashedPassword);
        }

        /// <summary>
        /// Valida se a senha atende aos critérios de segurança e correspondência com a confirmação.
        /// </summary>
        public static void Validate(string password, string? confirmPassword = "")
        {
            var validate = new DomainValidator();

            if (string.IsNullOrWhiteSpace(password))
                validate.AddError("A senha não pode estar vazia.");

            if (!string.IsNullOrEmpty(confirmPassword) && password != confirmPassword)
                validate.AddError("As senhas não correspondem.");

            if (password.Length < MIN_LENGTH || password.Length > MAX_LENGTH)
                validate.AddError($"A senha deve conter entre {MIN_LENGTH} e {MAX_LENGTH} caracteres.");

            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,24}$");
            if (!regex.IsMatch(password))
                validate.AddError("A senha deve conter letras maiúsculas, minúsculas, números e caracteres especiais.");

            validate.ThrowIfInvalid();
        }

        protected override IEnumerable<object?> GetValues()
        {
            yield return Value;
        }
    }
}
