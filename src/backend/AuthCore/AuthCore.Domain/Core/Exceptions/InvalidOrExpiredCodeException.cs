namespace AuthCore.Domain.Core.Exceptions
{
    /// <summary>Representa código inválido ou expirado.</summary>
    public sealed class InvalidOrExpiredCodeException : BadRequestException
    {
        /// <summary>Operação para criar instância de exceção.</summary>
        public InvalidOrExpiredCodeException()
            : base("Código inválido ou expirado.")
        {
        }
    }
}
