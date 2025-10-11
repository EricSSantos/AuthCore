namespace AuthCore.Domain.Commons.Interfaces.Security
{
    public interface IEntropy
    {
        (string Raw, string Hash) GeneratePair(int size = 64);
        string Hash(string raw);
        bool Verify(string raw, string hash);
    }
}
