using System.Net;

namespace AuthCore.Domain.Core.Exceptions
{
    /// <summary>
    /// Representa um erro de conflito de recurso (HTTP 409).
    /// </summary>
    public sealed class ConflictException : DomainException
    {
        public override HttpStatusCode StatusCode
        {
            get { return HttpStatusCode.Conflict; }
        }

        public override string Title
        {
            get { return "Conflito de recurso."; }
        }

        public ConflictException()
            : base("O recurso já existe.")
        { }

        public ConflictException(string message)
            : base(message)
        { }
    }
}
