using System.Net;

namespace AuthCore.Domain.Core.Exceptions
{
    /// <summary>Representa excesso de requisições.</summary>
    public class TooManyRequestsException : DomainException
    {
        private readonly HttpStatusCode _statusCode = HttpStatusCode.TooManyRequests;
        private readonly string _title = "Muitas requisições.";

        public override HttpStatusCode StatusCode
        {
            get { return _statusCode; }
        }

        public override string Title
        {
            get { return _title; }
        }

        /// <summary>Operação para criar instância de exceção.</summary>
        /// <param name="message">Mensagem do erro.</param>
        public TooManyRequestsException(string message)
            : base(message)
        {
        }
    }
}
