using System.Net;

namespace AuthCore.Domain.Shared
{
    /// <summary>
    /// Representa uma exceção de violação de regra de negócio no domínio.
    /// </summary>
    public abstract class DomainException : Exception
    {
        /// <summary>
        /// Lista de mensagens de erro associadas à exceção.
        /// </summary>
        public IReadOnlyCollection<string> Errors { get; }

        private readonly HttpStatusCode _statusCode = HttpStatusCode.BadRequest;
        private readonly string _title = "Violação na regra de negócio.";

        /// <summary>
        /// Código de status HTTP correspondente à exceção.
        /// </summary>
        public virtual HttpStatusCode StatusCode
        {
            get { return _statusCode; }
        }

        /// <summary>
        /// Título descritivo padrão da exceção.
        /// </summary>
        public virtual string Title
        {
            get { return _title; }
        }

        /// <summary>
        /// Cria uma nova instância da exceção com uma única mensagem.
        /// </summary>
        protected DomainException(string message)
            : base(message)
        {
            Errors = new[] { message };
        }

        /// <summary>
        /// Cria uma nova instância da exceção com uma coleção de mensagens.
        /// </summary>
        protected DomainException(IEnumerable<string> errors, Exception? innerException = null)
            : base(errors != null && errors.Any()
                  ? string.Join("; ", errors)
                  : "Violação na regra de negócio.", innerException)
        {
            Errors = errors != null ? errors.ToList() : new List<string>();
        }
    }
}
