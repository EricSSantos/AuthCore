using System.Net;

namespace AuthCore.Domain.Commons.Exceptions
{
    public class ConflictException : DomainException
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
        {
        }

        public ConflictException(string message)
            : base(message)
        {
        }
    }
}
