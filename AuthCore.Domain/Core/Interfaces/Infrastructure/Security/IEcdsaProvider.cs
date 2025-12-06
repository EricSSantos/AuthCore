namespace AuthCore.Domain.Core.Interfaces.Infrastructure.Security
{
    /// <summary>
    /// Define acesso às chaves ECDSA utilizadas em assinaturas digitais.
    /// </summary>
    public interface IEcdsaProvider
    {
        /// <summary>
        /// Obtém a chave privada para assinatura.
        /// </summary>
        byte[] PrivateKey { get; }

        /// <summary>
        /// Obtém a chave pública para validação.
        /// </summary>
        byte[] PublicKey { get; }
    }
}
