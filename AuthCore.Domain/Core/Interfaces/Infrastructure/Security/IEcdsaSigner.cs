namespace AuthCore.Domain.Core.Interfaces.Infrastructure.Security
{
    /// <summary>Define operações para assinar e validar dados com ECDSA.</summary>
    public interface IEcdsaSigner
    {
        /// <summary>Operação para gerar assinatura digital.</summary>
        /// <param name="text">Conteúdo a assinar.</param>
        string Sign(string text);

        /// <summary>Operação para verificar assinatura digital.</summary>
        /// <param name="text">Conteúdo original.</param>
        /// <param name="base64Signature">Assinatura digital em Base64.</param>
        bool Verify(string text, string base64Signature);
    }
}
