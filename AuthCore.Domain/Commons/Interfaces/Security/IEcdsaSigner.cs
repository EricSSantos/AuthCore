namespace AuthCore.Domain.Commons.Interfaces.Security
{
    public interface IEcdsaSigner
    {
        string Sign(string text);
        bool Verify(string text, string base64Signature);
    }
}
