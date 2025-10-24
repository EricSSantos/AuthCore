using AuthCore.Domain.Shared;
using System.Net;

namespace AuthCore.Domain.Commons.Exceptions
{
    public sealed class UnauthorizedException : DomainException
    {
        public override HttpStatusCode StatusCode
        {
            get { return HttpStatusCode.Unauthorized; }
        }

        public override string Title
        {
            get { return "Acesso não autorizado."; }
        }

        public UnauthorizedException()
            : base("Você não tem autorização para acessar este recurso.")
        { }

        public UnauthorizedException(string message)
            : base(message)
        { }
    }
}
