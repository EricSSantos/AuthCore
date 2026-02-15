using System.Net;

namespace AuthCore.Domain.Core.Exceptions
{
    /// <summary>Representa uma exceção de regra de negócio no domínio.</summary>
    public abstract class DomainException : Exception
    {
        public IReadOnlyCollection<string> Errors { get; }
        private readonly HttpStatusCode _statusCode = HttpStatusCode.BadRequest;
        private readonly string _title = "Violação na regra de negócio.";

        public virtual HttpStatusCode StatusCode
        {
            get { return _statusCode; }
        }

        public virtual string Title
        {
            get { return _title; }
        }

        /// <summary>Operação para criar instância de exceção.</summary>
        /// <param name="message">Mensagem do erro.</param>
        protected DomainException(string message)
            : base(message)
        {
            Errors = new[] { message };
        }

        /// <summary>Operação para criar instância de exceção.</summary>
        /// <param name="errors">Mensagens do erro.</param>
        /// <param name="innerException">Exceção interna.</param>
        protected DomainException(IEnumerable<string> errors, Exception? innerException = null)
            : base(errors != null && errors.Any()
                  ? string.Join("; ", errors)
                  : "Violação na regra de negócio.", innerException)
        {
            Errors = errors?.ToList() ?? new List<string>();
        }
    }
}
