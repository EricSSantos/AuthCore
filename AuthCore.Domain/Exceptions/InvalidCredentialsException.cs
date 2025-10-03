using System.Net;

namespace AuthCore.Domain.Exceptions
{
    public class InvalidCredentialsException : DomainException
    {
        public InvalidCredentialsException()
            : base("E-mail ou senha inválidos.") { }

        public InvalidCredentialsException(string message)
            : base(message) { }

        public override HttpStatusCode StatusCode => HttpStatusCode.Unauthorized;
        public override string Title => "Credenciais inválidas";
    }
}
