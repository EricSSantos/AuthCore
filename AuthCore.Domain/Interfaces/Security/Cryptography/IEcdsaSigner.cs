namespace AuthCore.Domain.Interfaces.Security.Cryptography
{
    public interface IEcdsaSigner
    {
        string Sign(string text);
        bool Verify(string text, string base64Signature);
    }
}
