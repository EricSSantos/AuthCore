using System.Net;

namespace AuthCore.Domain.Core.Exceptions
{
    /// <summary>
    /// Representa um erro de requisição inválida (HTTP 400).
    /// </summary>
    public sealed class BadRequestException : DomainException
    {
        public override HttpStatusCode StatusCode
        {
            get { return HttpStatusCode.BadRequest; }
        }

        public override string Title
        {
            get { return "Requisição inválida."; }
        }

        public BadRequestException(string message)
            : base(message)
        { }
    }
}
