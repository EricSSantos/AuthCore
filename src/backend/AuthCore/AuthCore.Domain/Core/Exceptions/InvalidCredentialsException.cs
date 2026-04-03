namespace AuthCore.Domain.Core.Exceptions
{
    /// <summary>Representa credenciais inválidas.</summary>
    public sealed class InvalidCredentialsException : UnauthorizedException
    {
        /// <summary>Operação para criar instância de exceção.</summary>
        public InvalidCredentialsException()
            : base("E-mail ou senha inválidos.")
        {
        }
    }
}
