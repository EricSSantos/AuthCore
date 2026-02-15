using System;
using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Base;
using System.Text.RegularExpressions;

namespace AuthCore.Domain.Aggregates.Users
{
    /// <summary>Representa uma senha criptografada do usuário.</summary>
    public sealed class Password : IValueObject
    {
        private const int MIN_LENGTH = 8;
        private const int MAX_LENGTH = 24;

        public string Value { get; }

        /// <summary>Operação para criar instância de senha.</summary>
        /// <param name="hashedPassword">Senha criptografada.</param>
        private Password(string hashedPassword)
        {
            Value = hashedPassword;
        }

        /// <summary>Operação para criar instância de senha.</summary>
        private Password()
        {
            Value = string.Empty;
        }

        /// <summary>Operação para criar senha criptografada.</summary>
        /// <param name="hashedPassword">Senha criptografada.</param>
        public static Password Create(string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(hashedPassword))
                throw new BadRequestException("A senha criptografada não pode estar vazia.");

            return new Password(hashedPassword);
        }

        /// <summary>Operação para validar senha.</summary>
        /// <param name="password">Senha a validar.</param>
        public static void Validate(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new BadRequestException("A senha não pode estar vazia.");

            if (password.Length < MIN_LENGTH || password.Length > MAX_LENGTH)
                throw new BadRequestException($"A senha deve conter entre {MIN_LENGTH} e {MAX_LENGTH} caracteres.");

            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,24}$");

            if (!regex.IsMatch(password))
                throw new BadRequestException("A senha deve conter letras maiúsculas, minúsculas, números e caracteres especiais.");
        }

        /// <summary>Operação para validar e confirmar senha.</summary>
        /// <param name="password">Senha informada.</param>
        /// <param name="confirmPassword">Confirmação da senha.</param>
        public static void ValidateWithConfirmation(string password, string confirmPassword)
        {
            Validate(password);

            if (string.IsNullOrWhiteSpace(confirmPassword))
                throw new BadRequestException("A confirmação de senha não pode estar vazia.");

            if (password != confirmPassword)
                throw new BadRequestException("As senhas não correspondem.");
        }
    }
}
