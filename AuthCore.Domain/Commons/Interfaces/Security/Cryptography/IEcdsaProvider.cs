namespace AuthCore.Domain.Commons.Interfaces.Security.Cryptography
{
    public interface IEcdsaProvider
    {
        byte[] PrivateKey { get; }
        byte[] PublicKey { get; }
    }
}
