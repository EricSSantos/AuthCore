namespace AuthCore.Domain.Core.Exceptions
{
    /// <summary>Representa senha inválida.</summary>
    public sealed class InvalidPasswordException : BadRequestException
    {
        /// <summary>Operação para criar instância de exceção.</summary>
        /// <param name="message">Mensagem do erro.</param>
        public InvalidPasswordException(string message)
            : base(message)
        {
        }
    }
}
