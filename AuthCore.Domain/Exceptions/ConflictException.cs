using System.Net;

namespace AuthCore.Domain.Exceptions
{
    public class ConflictException : DomainException
    {
        public ConflictException()
            : base("O recurso já existe.") { }

        public ConflictException(string message)
            : base(message) { }

        public override HttpStatusCode StatusCode => HttpStatusCode.Conflict;
        public override string Title => "Conflito de recurso";
    }
}
