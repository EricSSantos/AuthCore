using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Base;
using System.Text.RegularExpressions;

namespace AuthCore.Domain.Aggregates.Users
{
    /// <summary>Representa um endereço de e-mail validado.</summary>
    public sealed class Email : IValueObject
    {
        public string Value { get; }

        #region Constructors

        /// <summary>Operação para criar instância de e-mail.</summary>
        /// <param name="value">Valor do e-mail.</param>
        private Email(string value)
        {
            Value = value;
        }

        /// <summary>Operação para criar instância de e-mail.</summary>
        private Email()
        {
            Value = string.Empty;
        }

        #endregion

        #region Factory

        /// <summary>Operação para criar e-mail.</summary>
        /// <param name="email">Endereço de e-mail informado.</param>
        public static Email Create(string email)
        {
            Validate(email);
            return new Email(Normalize(email));
        }

        #endregion

        /// <summary>Operação para mascarar e-mail.</summary>
        public string Mask()
        {
            var parts = Value.Split('@');
            var local = parts[0];
            var domain = parts[1];

            if (local.Length <= 2)
                return $"{local[0]}***@{domain}";

            var visible = local[..2];
            return $"{visible}***@{domain}";
        }

        /// <summary>Operação para retornar e-mail sem máscara.</summary>
        public string Unmask()
        {
            return Value;
        }

        /// <summary>Operação para normalizar e-mail.</summary>
        /// <param name="email">E-mail a normalizar.</param>
        public static string Normalize(string email)
        {
            return email.Trim().ToLowerInvariant();
        }

        #region Validation

        /// <summary>Operação para validar e-mail.</summary>
        /// <param name="email">E-mail a validar.</param>
        public static void Validate(string email)
        {
            List<string> errors = new();
            var regex = new Regex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$");

            if (string.IsNullOrWhiteSpace(email))
                errors.Add("O e-mail não pode estar vazio.");

            if (!regex.IsMatch(Normalize(email)))
                errors.Add("O e-mail informado é inválido.");

            if (errors.Count > 0)
                throw new BadRequestException(errors);
        }

        #endregion
    }
}
