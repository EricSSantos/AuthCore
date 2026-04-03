using System.Net;

namespace AuthCore.Domain.Core.Exceptions
{
    /// <summary>Representa um erro de conflito de recurso.</summary>
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

        /// <summary>Operação para criar instância de exceção.</summary>
        public ConflictException()
            : base("O recurso já existe.")
        { }

        /// <summary>Operação para criar instância de exceção.</summary>
        /// <param name="message">Mensagem do erro.</param>
        public ConflictException(string message)
            : base(message)
        { }
    }
}
