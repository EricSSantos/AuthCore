namespace AuthCore.Domain.Commons.Interfaces.Security.Cryptography
{
    public interface IHmac
    {
        string ComputeBase64(byte[] data);
        bool Verify(string raw, string expected);
    }
}
