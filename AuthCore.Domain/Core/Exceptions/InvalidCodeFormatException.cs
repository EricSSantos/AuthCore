namespace AuthCore.Domain.Core.Exceptions
{
    /// <summary>Representa formato inválido de código.</summary>
    public sealed class InvalidCodeFormatException : BadRequestException
    {
        /// <summary>Operação para criar instância de exceção.</summary>
        /// <param name="message">Mensagem do erro.</param>
        public InvalidCodeFormatException(string message)
            : base(message)
        {
        }
    }
}
