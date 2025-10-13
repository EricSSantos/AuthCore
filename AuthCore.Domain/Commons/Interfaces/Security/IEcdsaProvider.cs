namespace AuthCore.Domain.Commons.Interfaces.Security
{
    public interface IEcdsaProvider
    {
        /// <summary>
        /// Obtém a chave privada usada para assinar tokens.
        /// </summary>
        byte[] PrivateKey { get; }

        /// <summary>
        /// Obtém a chave pública usada para validar assinaturas.
        /// </summary>
        byte[] PublicKey { get; }
    }
}
