using AuthCore.Domain.Core.Interfaces.Infrastructure.Security;
using AuthCore.Domain.Core.Settings;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace AuthCore.Infrastructure.Security
{
    /// <summary>Representa serviço de assinatura e provedor ECDSA.</summary>
    public sealed class EcdsaSigner : IEcdsaProvider
    {
        private const string PRIVATE_KEY = "PRIVATE KEY";
        private const string PUBLIC_KEY = "PUBLIC KEY";
        private readonly string _publicKeyPem;
        private readonly string _privateKeyPem;

        /// <summary>Operação para criar instância do assinador e provedor ECDSA.</summary>
        /// <param name="settings">Configurações de segurança.</param>
        public EcdsaSigner(IOptions<SecuritySettings> settings)
        {
            string publicKeyPath = settings.Value.Keys.Asymmetric.PublicKeyPath;
            string privateKeyPath = settings.Value.Keys.Asymmetric.PrivateKeyPath;

            if (!File.Exists(publicKeyPath) || !File.Exists(privateKeyPath))
                throw new ArgumentException("A chave pública ou privada não foi encontrada.");

            _publicKeyPem = File.ReadAllText(publicKeyPath);
            _privateKeyPem = File.ReadAllText(privateKeyPath);

            if (string.IsNullOrWhiteSpace(_publicKeyPem) || string.IsNullOrWhiteSpace(_privateKeyPem))
                throw new ArgumentException("A chave pública ou privada está vazia.");
        }

        #region Provider

        /// <summary>Operação para obter chave privada em formato DER.</summary>
        public byte[] PrivateKey
        {
            get { return PemToDer(_privateKeyPem, PRIVATE_KEY); }
        }

        /// <summary>Operação para obter chave pública em formato DER.</summary>
        public byte[] PublicKey
        {
            get { return PemToDer(_publicKeyPem, PUBLIC_KEY); }
        }

        #endregion

        #region Helpers

        /// <summary>Operação para converter chave PEM em formato DER.</summary>
        /// <param name="pem">Conteúdo da chave em formato PEM.</param>
        /// <param name="keyType">Tipo da chave.</param>
        private static byte[] PemToDer(string pem, string keyType)
        {
            string header = "-----BEGIN " + keyType + "-----";
            string footer = "-----END " + keyType + "-----";

            int start = pem.IndexOf(header, StringComparison.Ordinal);
            if (start < 0)
            {
                header = "-----BEGIN EC " + keyType + "-----";
                footer = "-----END EC " + keyType + "-----";
                start = pem.IndexOf(header, StringComparison.Ordinal);

                if (start < 0)
                    throw new ArgumentException(keyType + " cabeçalho não encontrado no PEM.");
            }

            start += header.Length;

            int end = pem.IndexOf(footer, start, StringComparison.Ordinal);
            if (end < 0)
                throw new ArgumentException(keyType + " rodapé não encontrado no PEM.");

            string base64 = pem.Substring(start, end - start)
                .Replace("\r", string.Empty)
                .Replace("\n", string.Empty)
                .Trim();

            return Convert.FromBase64String(base64);
        }

        #endregion
    }
}
