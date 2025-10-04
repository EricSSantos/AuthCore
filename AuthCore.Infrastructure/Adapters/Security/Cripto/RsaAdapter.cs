using AuthCore.Domain.Interfaces.Adapters.Security.Cripto;
using AuthCore.Domain.Settings;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace AuthCore.Infrastructure.Adapters.Security.Cripto
{
    public sealed class RsaAdapter : IRsaAdapter
    {
        #region Constants

        private const string PRIVATE_KEY = "PRIVATE KEY";
        private const string PUBLIC_KEY = "PUBLIC KEY";

        #endregion

        private readonly string _publicKey;
        private readonly string _privateKey;

        public RsaAdapter(IOptions<SecuritySettings> settings)
        {
            var asymmetric = settings.Value.Keys.Asymmetric;

            if (!File.Exists(asymmetric.PublicKeyPath))
                throw new ArgumentException("Chave pública não encontrada.");

            if (!File.Exists(asymmetric.PrivateKeyPath))
                throw new ArgumentException("Chave privada não encontrada.");

            _publicKey = File.ReadAllText(asymmetric.PublicKeyPath);
            _privateKey = File.ReadAllText(asymmetric.PrivateKeyPath);

            if (string.IsNullOrWhiteSpace(_publicKey))
                throw new ArgumentException("A chave pública PEM está vazia.");

            if (string.IsNullOrWhiteSpace(_privateKey))
                throw new ArgumentException("A chave privada PEM está vazia.");
        }

        public string Encrypt(string text)
        {
            using var rsa = RSA.Create();
            var pubDer = PemToDer(_publicKey, PUBLIC_KEY);
            rsa.ImportSubjectPublicKeyInfo(pubDer, out _);

            var data = Encoding.UTF8.GetBytes(text);
            var encrypted = rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);

            return Convert.ToBase64String(encrypted);
        }

        public string Decrypt(string base64)
        {
            using var rsa = RSA.Create();
            var privDer = PemToDer(_privateKey, PRIVATE_KEY);
            rsa.ImportPkcs8PrivateKey(privDer, out _);

            var cipherBytes = Convert.FromBase64String(base64);
            var decrypted = rsa.Decrypt(cipherBytes, RSAEncryptionPadding.OaepSHA256);

            return Encoding.UTF8.GetString(decrypted);
        }

        public byte[] GetPrivateKey()
        {
            return PemToDer(_privateKey, PRIVATE_KEY);
        }

        public byte[] GetPublicKey()
        {
            return PemToDer(_publicKey, PUBLIC_KEY);
        }

        private static byte[] PemToDer(string pem, string key)
        {
            var header = $"-----BEGIN {key}-----";
            var footer = $"-----END {key}-----";

            var start = pem.IndexOf(header, StringComparison.Ordinal);
            if (start < 0)
                throw new ArgumentException($"{key} cabeçalho não encontrado no PEM.");

            start += header.Length;

            var end = pem.IndexOf(footer, start, StringComparison.Ordinal);
            if (end < 0)
                throw new ArgumentException($"{key} rodapé não encontrado no PEM.");

            var base64 = pem.Substring(start, end - start);
            base64 = base64.Replace("\r", "").Replace("\n", "").Trim();

            return Convert.FromBase64String(base64);
        }
    }
}
