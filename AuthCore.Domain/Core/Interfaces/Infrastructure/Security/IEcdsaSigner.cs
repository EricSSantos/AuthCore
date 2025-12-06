namespace AuthCore.Domain.Core.Interfaces.Infrastructure.Security
{
    /// <summary>
    /// Define operações para assinar e validar dados com ECDSA.
    /// </summary>
    public interface IEcdsaSigner
    {
        /// <summary>
        /// Gera a assinatura digital do texto.
        /// </summary>
        /// <param name="text">Conteúdo a assinar.</param>
        /// <returns>Assinatura em Base64.</returns>
        string Sign(string text);

        /// <summary>
        /// Verifica se a assinatura corresponde ao texto.
        /// </summary>
        /// <param name="text">Conteúdo original.</param>
        /// <param name="base64Signature">Assinatura digital em Base64.</param>
        /// <returns>True quando a assinatura é válida.</returns>
        bool Verify(string text, string base64Signature);
    }
}
