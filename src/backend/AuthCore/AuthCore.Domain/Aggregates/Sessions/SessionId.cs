using AuthCore.Domain.Core.Exceptions;
using AuthCore.Domain.Core.Interfaces.Base;

namespace AuthCore.Domain.Aggregates.Sessions
{
    /// <summary>Representa o identificador único de uma sessão (hash SHA-256).</summary>
    public sealed class SessionId : IValueObject
    {
        public string Value { get; }

        /// <summary>Operação para criar instância do identificador de sessão.</summary>
        /// <param name="value">Valor do identificador da sessão.</param>
        private SessionId(string value)
        {
            Value = value;
        }

        /// <summary>Operação para criar identificador de sessão.</summary>
        /// <param name="value">Valor do identificador da sessão.</param>
        public static SessionId Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new BadRequestException("O identificador de sessão é inválido.");

            return new SessionId(value.Trim());
        }

        /// <summary>Operação para retornar o identificador da sessão em formato textual.</summary>
        public override string ToString()
        {
            return Value;
        }
    }
}
