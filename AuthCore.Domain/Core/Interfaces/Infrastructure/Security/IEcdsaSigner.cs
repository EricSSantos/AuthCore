namespace AuthCore.Domain.Core.Interfaces.Infrastructure.Security
{
    /// <summary>
    /// Define métodos para assinar e validar dados usando ECDSA.
    /// </summary>
    public interface IEcdsaSigner
    {
        /// <summary>
        /// Cria uma assinatura digital para o texto informado.
        /// </summary>
        /// <param name="text">Conteúdo a ser assinado.</param>
        string Sign(string text);

        /// <summary>
        /// Verifica se a assinatura corresponde ao texto original.
        /// </summary>
        /// <param name="text">Conteúdo original.</param>
        /// <param name="base64Signature">Assinatura digital codificada em Base64.</param>
        bool Verify(string text, string base64Signature);
    }
}
