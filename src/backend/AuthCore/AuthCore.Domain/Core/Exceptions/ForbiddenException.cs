using System.Net;

namespace AuthCore.Domain.Core.Exceptions
{
    /// <summary>Representa um erro de acesso negado.</summary>
    public class ForbiddenException : DomainException
    {
        public override HttpStatusCode StatusCode
        {
            get { return HttpStatusCode.Forbidden; }
        }

        public override string Title
        {
            get { return "Acesso negado."; }
        }

        /// <summary>Operação para criar instância de exceção.</summary>
        public ForbiddenException()
            : base("Você não tem permissão para acessar este recurso.")
        { }

        /// <summary>Operação para criar instância de exceção.</summary>
        /// <param name="message">Mensagem do erro.</param>
        public ForbiddenException(string message)
            : base(message)
        { }
    }
}
