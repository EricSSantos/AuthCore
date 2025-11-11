using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Base;
using System.Text.RegularExpressions;

namespace AuthCore.Domain.Aggregates.UserAggregate
{
    /// <summary>
    /// Representa um endereço de e-mail validado do domínio.
    /// </summary>
    public sealed class Email : IValueObject
    {
        public string Value { get; }

        private Email(string value)
        {
            Value = value;
        }

        private Email() { }

        /// <summary>
        /// Cria uma nova instância de Email após validar e normalizar o valor.
        /// </summary>
        public static Email Create(string email)
        {
            Validate(email);
            return new Email(Normalize(email));
        }

        /// <summary>
        /// Mascara parte do e-mail para uso seguro.
        /// </summary>
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

        /// <summary>
        /// Retorna o e-mail completo sem máscara.
        /// </summary>
        public string Unmask()
        {
            return Value;
        }

        /// <summary>
        /// Normaliza o e-mail para comparação e armazenamento.
        /// </summary>
        public static string Normalize(string email)
        {
            return email.Trim().ToLowerInvariant();
        }

        /// <summary>
        /// Valida o formato do e-mail informado.
        /// </summary>
        /// <exception cref="BadRequestException"></exception>
        public static void Validate(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new BadRequestException("O e-mail não pode estar vazio.");

            var regex = new Regex(@"^[^\s@]+@[^\s@]+\.[^\s@]+$");

            if (!regex.IsMatch(Normalize(email)))
                throw new BadRequestException("O e-mail informado é inválido.");
        }
    }
}
