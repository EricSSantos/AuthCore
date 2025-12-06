namespace AuthCore.Domain.Core.Interfaces.Infrastructure.Security
{
    /// <summary>
    /// Define operações para gerar e validar hashes de senha.
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Gera o hash seguro da senha.
        /// </summary>
        /// <param name="plainText">Senha em texto simples.</param>
        /// <returns>Hash gerado.</returns>
        string Hash(string plainText);

        /// <summary>
        /// Verifica se a senha corresponde ao hash.
        /// </summary>
        /// <param name="plainText">Senha digitada.</param>
        /// <param name="hash">Hash armazenado.</param>
        /// <returns>True quando os valores coincidem.</returns>
        bool IsValid(string plainText, string hash);
    }
}
