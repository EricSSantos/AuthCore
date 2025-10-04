namespace AuthCore.Domain.Interfaces.Adapters.Security.Cripto
{
    public interface IBCryptAdapter
    {
        string Hash(string plainText);
        bool isValid(string plainText, string hash);
    }
}
