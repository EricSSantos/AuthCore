namespace AuthCore.Domain.Core.Interfaces.Infrastructure.Security
{
    /// <summary>Define operações para obtenção das chaves ECDSA utilizadas em assinaturas digitais.</summary>
    public interface IEcdsaProvider
    {
        /// <summary>Operação para obter chave privada em formato DER.</summary>
        byte[] PrivateKey { get; }

        /// <summary>Operação para obter chave pública em formato DER.</summary>
        byte[] PublicKey { get; }
    }
}