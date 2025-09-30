using System.Net;

namespace AuthCore.Domain.Exceptions
{
    public class NotFoundException : DomainException
    {
        public NotFoundException()
            : base("O recurso solicitado não foi encontrado.") { }

        public NotFoundException(string message)
            : base(message) { }

        public override HttpStatusCode StatusCode => HttpStatusCode.NotFound;
        public override string Title => "Recurso não encontrado";
    }
}
