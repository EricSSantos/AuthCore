using AuthCore.Domain.Shared;
using System.Net;

namespace AuthCore.Domain.Commons.Exceptions
{
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

        public BadRequestException(IEnumerable<string> errors)
            : base(errors)
        { }
    }
}
