using System.Net;

namespace AuthCore.Domain.Core.Exceptions
{
    /// <summary>
    /// Representa um erro de acesso negado (HTTP 403).
    /// </summary>
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
