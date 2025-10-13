namespace AuthCore.Domain.Commons.Interfaces.Helpers
{
    public interface IJsonSerializer
    {
        /// <summary>
        /// Converte um objeto em uma string JSON.
        /// </summary>
        string Serialize<T>(T obj);

        /// <summary>
        /// Converte uma string JSON em um objeto do tipo especificado.
        /// </summary>
        T? Deserialize<T>(string json);
    }
}
