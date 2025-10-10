using AuthCore.Domain.Commons.Interfaces.Helpers;

namespace AuthCore.Infrastructure.Helpers
{
    public sealed class JsonSerializer : IJsonSerializer
    {
        public string Serialize<T>(T obj)
        {
            return System.Text.Json.JsonSerializer.Serialize(obj);
        }

        public T? Deserialize<T>(string json)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(json);
        }
    }
}
