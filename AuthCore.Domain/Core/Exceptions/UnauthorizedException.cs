using System.Net;

namespace AuthCore.Domain.Core.Exceptions
{
    /// <summary>Representa um erro de autenticação inválida.</summary>
    public class UnauthorizedException : DomainException
    {
        public override HttpStatusCode StatusCode
        {
            get { return HttpStatusCode.Unauthorized; }
        }

        public override string Title
        {
            get { return "Acesso não autorizado."; }
        }

        /// <summary>Operação para criar instância de exceção.</summary>
        public UnauthorizedException()
            : base("Você não tem autorização para acessar este recurso.")
        { }

        /// <summary>Operação para criar instância de exceção.</summary>
        /// <param name="message">Mensagem do erro.</param>
        public UnauthorizedException(string message)
            : base(message)
        { }
    }
}
