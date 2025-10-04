namespace AuthCore.Domain.Interfaces.Adapters.Security.Cripto
{
    public interface IRsaAdapter
    {
        string Encrypt(string text);
        string Decrypt(string base64);
        byte[] GetPrivateKey();
        byte[] GetPublicKey();
    }
}
