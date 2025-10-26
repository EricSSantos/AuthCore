namespace AuthCore.Domain.Core.Interfaces.Security
{
    /// <summary>
    /// Define métodos para gerar chaves seguras.
    /// </summary>
    public interface ISecureKeyGenerator
    {
        /// <summary>
        /// Cria uma string aleatória segura em formato hexadecimal.
        /// </summary>
        /// <param name="size">Tamanho do buffer em bytes (padrão: 32).</param>
        string Generate(int size = 32);

        /// <summary>
        /// Gera um hash HMAC-SHA256 a partir do valor informado.
        /// </summary>
        /// <param name="raw">Valor original a ser transformado em hash.</param>
        string Hash(string raw);

        /// <summary>
        /// Verifica se o valor corresponde ao hash gerado.
        /// </summary>
        /// <param name="raw">Valor original.</param>
        /// <param name="hash">Hash armazenado para comparação.</param>
        bool Verify(string raw, string hash);
    }
}
