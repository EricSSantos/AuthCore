namespace AuthCore.Domain.Core.Interfaces.Infrastructure.Security
{
    /// <summary>Define acesso às chaves ECDSA utilizadas em assinaturas digitais.</summary>
    public interface IEcdsaProvider
    {
        byte[] PrivateKey { get; }

        byte[] PublicKey { get; }
    }
}
