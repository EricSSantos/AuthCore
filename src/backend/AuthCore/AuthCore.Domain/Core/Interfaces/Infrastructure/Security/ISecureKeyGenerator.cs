namespace AuthCore.Domain.Core.Interfaces.Infrastructure.Security
{
    /// <summary>Define operações para gerar chaves e hashes seguros.</summary>
    public interface ISecureKeyGenerator
    {
        /// <summary>Operação para gerar chave segura em formato hexadecimal.</summary>
        /// <param name="size">Tamanho da chave em bytes.</param>
        string Generate(int size = 32);

        /// <summary>Operação para gerar hash HMAC da chave informada.</summary>
        /// <param name="raw">Chave em formato hexadecimal.</param>
        string Hash(string raw);

        /// <summary>Operação para validar hash HMAC da chave informada.</summary>
        /// <param name="raw">Chave em formato hexadecimal.</param>
        /// <param name="hash">Hash armazenado.</param>
        bool Verify(string raw, string hash);
    }
}