namespace AuthCore.Domain.Commons.Interfaces.Security.Signing
{
    public interface IHmac
    {
        /// <summary>
        /// Calcula o HMAC e retorna o resultado em formato Base64.
        /// </summary>
        string ComputeBase64(byte[] data);

        /// <summary>
        /// Verifica se o HMAC gerado corresponde ao valor esperado.
        /// </summary>
        bool Verify(string raw, string expected);
    }
}
