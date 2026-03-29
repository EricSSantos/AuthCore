using System.Net;

namespace AuthCore.Domain.Core.Exceptions
{
    /// <summary>Representa um erro de requisição inválida.</summary>
    public class BadRequestException : DomainException
    {
        public override HttpStatusCode StatusCode
        {
            get { return HttpStatusCode.BadRequest; }
        }

        public override string Title
        {
            get { return "Requisição inválida."; }
        }

        /// <summary>Operação para criar instância de exceção.</summary>
        /// <param name="message">Mensagem do erro.</param>
        public BadRequestException(string message)
            : base(message)
        { }

        /// <summary>Operação para criar instância de exceção.</summary>
        /// <param name="errors">Mensagens do erro.</param>
        public BadRequestException(IEnumerable<string> errors)
            : base(errors)
        { }
    }
}
