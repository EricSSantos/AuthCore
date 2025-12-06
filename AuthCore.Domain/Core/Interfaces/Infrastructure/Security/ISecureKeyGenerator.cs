namespace AuthCore.Domain.Core.Interfaces.Infrastructure.Security
{
    /// <summary>
    /// Define operações para gerar chaves e hashes seguros.
    /// </summary>
    public interface ISecureKeyGenerator
    {
        /// <summary>
        /// Gera string aleatória segura em hexadecimal.
        /// </summary>
        /// <param name="size">Tamanho do buffer em bytes.</param>
        /// <returns>Chave gerada.</returns>
        string Generate(int size = 32);

        /// <summary>
        /// Gera hash HMAC-SHA256 do valor informado.
        /// </summary>
        /// <param name="raw">Valor original.</param>
        /// <returns>Hash gerado.</returns>
        string Hash(string raw);

        /// <summary>
        /// Verifica se o valor corresponde ao hash.
        /// </summary>
        /// <param name="raw">Valor original.</param>
        /// <param name="hash">Hash armazenado.</param>
        /// <returns>True quando os valores coincidem.</returns>
        bool Verify(string raw, string hash);
    }
}
