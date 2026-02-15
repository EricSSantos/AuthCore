namespace AuthCore.Domain.Core.Exceptions
{
    /// <summary>Representa conta temporariamente bloqueada.</summary>
    public sealed class AccountLockedException : UnauthorizedException
    {
        /// <summary>Operação para criar instância de exceção.</summary>
        /// <param name="message">Mensagem do bloqueio.</param>
        public AccountLockedException(string message)
            : base(message)
        {
        }
    }
}
