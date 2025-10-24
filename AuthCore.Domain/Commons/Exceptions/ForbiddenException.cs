using AuthCore.Domain.Shared;
using System.Net;

namespace AuthCore.Domain.Commons.Exceptions
{
    public sealed class ForbiddenException : DomainException
    {
        public override HttpStatusCode StatusCode
        {
            get { return HttpStatusCode.Forbidden; }
        }

        public override string Title
        {
            get { return "Acesso negado."; }
        }

        public ForbiddenException()
            : base("Você não tem permissão para acessar este recurso.")
        { }

        public ForbiddenException(string message)
            : base(message)
        { }
    }
}
