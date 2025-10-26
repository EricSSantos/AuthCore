namespace AuthCore.Domain.Core.Settings
{
    /// <summary>
    /// Define as configurações gerais de segurança da aplicação.
    /// </summary>
    public sealed class SecuritySettings
    {
        /// <summary>
        /// Configurações relacionadas à geração e validação de tokens JWT.
        /// </summary>
        public JwtSettings Jwt { get; set; } = new();

        /// <summary>
        /// Configurações relacionadas à duração e controle das sessões de usuário.
        /// </summary>
        public SessionSettings Session { get; set; } = new();

        /// <summary>
        /// Configurações das chaves criptográficas usadas para assinatura e verificação.
        /// </summary>
        public SecurityKeysSettings Keys { get; set; } = new();
    }

    /// <summary>
    /// Define as informações de configuração do JWT.
    /// </summary>
    public sealed class JwtSettings
    {
        /// <summary>
        /// Identifica o emissor dos tokens JWT.
        /// </summary>
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// Identifica o público-alvo dos tokens JWT.
        /// </summary>
        public string Audience { get; set; } = string.Empty;

        /// <summary>
        /// Tempo de expiração do token JWT em minutos.
        /// </summary>
        public int ExpiresInMinutes { get; set; } = 30;
    }

    /// <summary>
    /// Define as configurações relacionadas à sessão do usuário.
    /// </summary>
    public sealed class SessionSettings
    {
        /// <summary>
        /// Tempo de expiração padrão da sessão em dias.
        /// </summary>
        public int ExpiresInDays { get; set; } = 7;

        /// <summary>
        /// Tempo máximo de vida útil da sessão em dias.
        /// </summary>
        public int MaxLifetimeInDays { get; set; } = 30;
    }

    /// <summary>
    /// Agrupa as configurações das chaves criptográficas.
    /// </summary>
    public sealed class SecurityKeysSettings
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
    /// Define a chave simétrica usada em algoritmos de criptografia.
    /// </summary>
    public sealed class SymmetricKeySettings
    {
        /// <summary>
        /// Chave privada utilizada em algoritmos simétricos.
        /// </summary>
        public string PrivateKey { get; set; } = string.Empty;
    }

    /// <summary>
    /// Define as chaves assimétricas usadas em ECDSA ou RSA.
    /// </summary>
    public sealed class AsymmetricKeySettings
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
