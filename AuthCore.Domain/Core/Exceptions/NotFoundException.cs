using System.Net;

namespace AuthCore.Domain.Core.Exceptions
{
    /// <summary>Representa um erro de recurso não encontrado.</summary>
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

        /// <summary>Operação para criar instância de exceção.</summary>
        public NotFoundException()
            : base("O recurso solicitado não foi encontrado.")
        { }

        /// <summary>Operação para criar instância de exceção.</summary>
        /// <param name="message">Mensagem do erro.</param>
        public NotFoundException(string message)
            : base(message)
        { }
    }
}
