namespace AuthCore.Domain.Core.Settings
{
    /// <summary>
    /// Define configurações gerais de segurança da aplicação.
    /// </summary>
    public sealed class SecuritySettings
    {
        /// <summary>
        /// Define parâmetros do JWT.
        /// </summary>
        public JwtSettings Jwt { get; set; } = new();

        /// <summary>
        /// Define parâmetros de sessão do usuário.
        /// </summary>
        public SessionSettings Session { get; set; } = new();

        /// <summary>
        /// Define configurações das chaves criptográficas.
        /// </summary>
        public SecurityKeysSettings Keys { get; set; } = new();
    }

    /// <summary>
    /// Define configurações do JWT.
    /// </summary>
    public sealed class JwtSettings
    {
        /// <summary>
        /// Define o emissor dos tokens.
        /// </summary>
        public string Issuer { get; set; } = string.Empty;

        /// <summary>
        /// Define o público dos tokens.
        /// </summary>
        public string Audience { get; set; } = string.Empty;

        /// <summary>
        /// Define o tempo de expiração em minutos.
        /// </summary>
        public int ExpiresInMinutes { get; set; } = 30;
    }

    /// <summary>
    /// Define configurações da sessão do usuário.
    /// </summary>
    public sealed class SessionSettings
    {
        /// <summary>
        /// Define expiração padrão da sessão em dias.
        /// </summary>
        public int ExpiresInDays { get; set; } = 7;

        /// <summary>
        /// Define o tempo máximo de vida útil em dias.
        /// </summary>
        public int MaxLifetimeInDays { get; set; } = 30;
    }

    /// <summary>
    /// Agrupa configurações das chaves criptográficas.
    /// </summary>
    public sealed class SecurityKeysSettings
    {
        /// <summary>
        /// Define configurações da chave simétrica.
        /// </summary>
        public SymmetricKeySettings Symmetric { get; set; } = new();

        /// <summary>
        /// Define configurações das chaves assimétricas.
        /// </summary>
        public AsymmetricKeySettings Asymmetric { get; set; } = new();
    }

    /// <summary>
    /// Define configurações da chave simétrica.
    /// </summary>
    public sealed class SymmetricKeySettings
    {
        /// <summary>
        /// Define a chave privada simétrica.
        /// </summary>
        public string PrivateKey { get; set; } = string.Empty;
    }

    /// <summary>
    /// Define configurações das chaves assimétricas.
    /// </summary>
    public sealed class AsymmetricKeySettings
    {
        /// <summary>
        /// Define o caminho da chave privada.
        /// </summary>
        public string PrivateKeyPath { get; set; } = string.Empty;

        /// <summary>
        /// Define o caminho da chave pública.
        /// </summary>
        public string PublicKeyPath { get; set; } = string.Empty;
    }
}
