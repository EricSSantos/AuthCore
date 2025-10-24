using AuthCore.Domain.Shared;
using System.Net;

namespace AuthCore.Domain.Commons.Exceptions
{
    public class NotFoundException : DomainException
    {
        public override HttpStatusCode StatusCode
        {
            get { return HttpStatusCode.NotFound; }
        }

        public override string Title
        {
            get { return "Recurso não encontrado."; }
        }

        public NotFoundException()
            : base("O recurso solicitado não foi encontrado.")
        { }

        public NotFoundException(string message)
            : base(message)
        { }
    }
}
