using System.Net;

namespace AuthCore.Domain.Commons.Exceptions
{
    public sealed class UnauthorizedException : DomainException
    {
        public UnauthorizedException()
            : base("O acesso não foi autorizado.") { }

        public UnauthorizedException(string message)
            : base(message) { }

        public override HttpStatusCode StatusCode => HttpStatusCode.Unauthorized;
        public override string Title => "Acesso não autorizado";
    }
}
