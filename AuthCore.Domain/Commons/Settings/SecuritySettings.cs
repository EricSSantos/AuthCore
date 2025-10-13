namespace AuthCore.Domain.Commons.Settings
{
    /// <summary>
    /// Representa as configurações de segurança da aplicação.
    /// </summary>
    public class SecuritySettings
    {
        /// <summary>
        /// Configurações relacionadas ao JWT (tokens de autenticação).
        /// </summary>
        public JwtSettings Jwt { get; set; } = new();

        /// <summary>
        /// Configurações das chaves criptográficas usadas para assinatura e validação.
        /// </summary>
        public SecurityKeysSettings Keys { get; set; } = new();
    }

    /// <summary>
    /// Define as informações básicas de configuração do JWT.
    /// </summary>
    public class JwtSettings
    {
        /// <summary>
        /// Identifica o emissor dos tokens JWT.
        /// </summary>
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// Identifica o público-alvo dos tokens JWT.
        /// </summary>
        public string Audience { get; set; } = string.Empty;
    }

    /// <summary>
    /// Agrupa as configurações de chaves simétricas e assimétricas.
    /// </summary>
    public class SecurityKeysSettings
    {
        /// <summary>
        /// Configurações da chave simétrica.
        /// </summary>
        public SymmetricKeySettings Symmetric { get; set; } = new();

        /// <summary>
        /// Configurações das chaves assimétricas (pública e privada).
        /// </summary>
        public AsymmetricKeySettings Asymmetric { get; set; } = new();
    }

    /// <summary>
    /// Define a configuração da chave simétrica usada em criptografia.
    /// </summary>
    public class SymmetricKeySettings
    {
        /// <summary>
        /// Chave privada utilizada em algoritmos simétricos.
        /// </summary>
        public string PrivateKey { get; set; } = string.Empty;
    }

    /// <summary>
    /// Define as configurações das chaves assimétricas usadas em ECDSA ou RSA.
    /// </summary>
    public class AsymmetricKeySettings
    {
        /// <summary>
        /// Caminho do arquivo contendo a chave privada.
        /// </summary>
        public string PrivateKeyPath { get; set; } = string.Empty;

        /// <summary>
        /// Caminho do arquivo contendo a chave pública.
        /// </summary>
        public string PublicKeyPath { get; set; } = string.Empty;
    }
}
