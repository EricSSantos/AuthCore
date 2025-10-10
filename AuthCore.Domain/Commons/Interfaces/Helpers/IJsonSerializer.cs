namespace AuthCore.Domain.Commons.Interfaces.Helpers
{
    public interface IJsonSerializer
    {
        string Serialize<T>(T obj);
        T? Deserialize<T>(string json);
    }
}
