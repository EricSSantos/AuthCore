namespace AuthCore.Domain.Commons.Interfaces.Security
{
    public interface IEcdsaProvider
    {
        byte[] PrivateKey { get; }
        byte[] PublicKey { get; }
    }
}
