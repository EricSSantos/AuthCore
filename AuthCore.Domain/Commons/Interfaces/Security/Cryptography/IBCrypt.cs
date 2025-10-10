namespace AuthCore.Domain.Commons.Interfaces.Security.Cryptography
{
    public interface IBCrypt
    {
        string Hash(string plainText);
        bool isValid(string plainText, string hash);
    }
}
