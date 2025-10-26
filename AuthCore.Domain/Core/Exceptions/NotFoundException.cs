using System.Net;

namespace AuthCore.Domain.Core.Exceptions
{
    /// <summary>
    /// Representa um erro quando o recurso solicitado não é encontrado (HTTP 404).
    /// </summary>
    public sealed class NotFoundException : DomainException
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
