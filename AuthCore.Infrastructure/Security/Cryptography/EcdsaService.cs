using AuthCore.Domain.Interfaces.Security.Cryptography;
using AuthCore.Domain.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace AuthCore.Infrastructure.Security.Cryptography
{
    public sealed class EcdsaService : IEcdsaSigner, IEcdsaProvider
    {
        #region Constants

        private const string PRIVATE_KEY = "PRIVATE KEY";
        private const string PUBLIC_KEY = "PUBLIC KEY";

        #endregion

        private readonly string _publicKeyPem;
        private readonly string _privateKeyPem;

        public EcdsaService(IOptions<SecuritySettings> settings)
        {
            var keys = settings.Value.Keys.Asymmetric;

            if (!File.Exists(keys.PublicKeyPath))
                throw new ArgumentException("Chave pública não encontrada no caminho especificado.");

            if (!File.Exists(keys.PrivateKeyPath))
                throw new ArgumentException("Chave privada não encontrada no caminho especificado.");

            _publicKeyPem = File.ReadAllText(keys.PublicKeyPath);
            _privateKeyPem = File.ReadAllText(keys.PrivateKeyPath);

            if (string.IsNullOrWhiteSpace(_publicKeyPem))
                throw new ArgumentException("A chave pública PEM está vazia.");

            if (string.IsNullOrWhiteSpace(_privateKeyPem))
                throw new ArgumentException("A chave privada PEM está vazia.");
        }

        #region Provider

        public byte[] PrivateKey
        {
            get { return PemToDer(_privateKeyPem, PRIVATE_KEY); }
        }

        public byte[] PublicKey
        {
            get { return PemToDer(_publicKeyPem, PUBLIC_KEY); }
        }

        #endregion

        #region Signer

        public string Sign(string text)
        {
            var data = Encoding.UTF8.GetBytes(text);
            var privateDer = PemToDer(_privateKeyPem, PRIVATE_KEY);

            using (var ecdsa = ECDsa.Create())
            {
                ecdsa.ImportPkcs8PrivateKey(privateDer, out _);

                var signature = ecdsa.SignData(
                    data,
                    HashAlgorithmName.SHA256,
                    DSASignatureFormat.IeeeP1363FixedFieldConcatenation
                );

                return Base64UrlEncoder.Encode(signature);
            }
        }

        public bool Verify(string text, string base64Signature)
        {
            if (string.IsNullOrWhiteSpace(base64Signature))
                return false;

            var data = Encoding.UTF8.GetBytes(text);
            var publicDer = PemToDer(_publicKeyPem, PUBLIC_KEY);

            using (var ecdsa = ECDsa.Create())
            {
                ecdsa.ImportSubjectPublicKeyInfo(publicDer, out _);

                var signatureBytes = Base64UrlEncoder.DecodeBytes(base64Signature);

                return ecdsa.VerifyData(
                    data,
                    signatureBytes,
                    HashAlgorithmName.SHA256,
                    DSASignatureFormat.IeeeP1363FixedFieldConcatenation
                );
            }
        }

        #endregion

        #region Private Methods

        private static byte[] PemToDer(string pem, string keyType)
        {
            var header = "-----BEGIN " + keyType + "-----";
            var footer = "-----END " + keyType + "-----";

            var start = pem.IndexOf(header, StringComparison.Ordinal);
            if (start < 0)
            {
                header = "-----BEGIN EC " + keyType + "-----";
                footer = "-----END EC " + keyType + "-----";
                start = pem.IndexOf(header, StringComparison.Ordinal);

                if (start < 0)
                    throw new ArgumentException(keyType + " cabeçalho não encontrado no PEM.");
            }

            start += header.Length;

            var end = pem.IndexOf(footer, start, StringComparison.Ordinal);
            if (end < 0)
                throw new ArgumentException(keyType + " rodapé não encontrado no PEM.");

            var base64 = pem.Substring(start, end - start)
                .Replace("\r", string.Empty)
                .Replace("\n", string.Empty)
                .Trim();

            return Convert.FromBase64String(base64);
        }

        #endregion
    }
}
