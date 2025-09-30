namespace AuthCore.Domain.Interfaces.Security
{
    public interface IBcryptHasher
    {
        string Hash(string plainText);
        bool isValid(string plainText, string hash);
    }
}
