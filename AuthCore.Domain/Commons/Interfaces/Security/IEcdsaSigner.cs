namespace AuthCore.Domain.Commons.Interfaces.Security
{
    public interface IEcdsaSigner
    {
        /// <summary>
        /// Gera uma assinatura digital para o texto informado.
        /// </summary>
        string Sign(string text);

        /// <summary>
        /// Verifica se a assinatura é válida para o texto informado.
        /// </summary>
        bool Verify(string text, string base64Signature);
    }
}
