namespace AuthCore.Domain.Core.Interfaces.Security
{
    /// <summary>
    /// Define métodos para gerar e validar hashes de senha.
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Gera um hash seguro a partir de um texto simples.
        /// </summary>
        /// <param name="plainText">Texto original (senha em texto simples).</param>
        string Hash(string plainText);

        /// <summary>
        /// Verifica se o texto informado corresponde ao hash armazenado.
        /// </summary>
        /// <param name="plainText">Texto original (senha digitada pelo usuário).</param>
        /// <param name="hash">Hash armazenado no banco de dados.</param>
        bool IsValid(string plainText, string hash);
    }
}
