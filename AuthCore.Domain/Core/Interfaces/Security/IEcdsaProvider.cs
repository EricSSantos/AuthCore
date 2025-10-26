namespace AuthCore.Domain.Core.Interfaces.Security
{
    /// <summary>
    /// Define propriedades para fornecer chaves ECDSA usadas em assinaturas digitais.
    /// </summary>
    public interface IEcdsaProvider
    {
        /// <summary>
        /// Chave privada usada para assinar tokens.
        /// </summary>
        byte[] PrivateKey { get; }

        /// <summary>
        /// Chave pública usada para validar assinaturas.
        /// </summary>
        byte[] PublicKey { get; }
    }
}
