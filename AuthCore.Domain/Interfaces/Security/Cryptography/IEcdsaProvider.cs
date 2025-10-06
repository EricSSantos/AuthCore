namespace AuthCore.Domain.Interfaces.Security.Cryptography
{
    public interface IEcdsaProvider
    {
        byte[] PrivateKey { get; }
        byte[] PublicKey { get; }
    }
}
