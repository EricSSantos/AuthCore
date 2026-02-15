namespace AuthCore.Domain.Core.Interfaces.Infrastructure.Security
{
    /// <summary>Define operações para gerar chaves e hashes seguros.</summary>
    public interface ISecureKeyGenerator
    {
        /// <summary>Operação para gerar chave segura.</summary>
        /// <param name="size">Tamanho do buffer em bytes.</param>
        string Generate(int size = 32);

        /// <summary>Operação para gerar hash seguro.</summary>
        /// <param name="raw">Valor original.</param>
        string Hash(string raw);

        /// <summary>Operação para verificar hash seguro.</summary>
        /// <param name="raw">Valor original.</param>
        /// <param name="hash">Hash armazenado.</param>
        bool Verify(string raw, string hash);
    }
}
