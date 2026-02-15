namespace AuthCore.Domain.Core.Settings
{
    /// <summary>Representa configurações de conexão com bancos.</summary>
    public class DatabaseSettings
    {
        public PostgresSettings Postgres { get; set; } = new();

        public RedisSettings Redis { get; set; } = new();
    }

    /// <summary>Representa parâmetros de conexão do PostgreSQL.</summary>
    public class PostgresSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public int CommandTimeoutSeconds { get; set; } = 30;
        public int RetryCount { get; set; } = 3;
        public int RetryDelayMilliseconds { get; set; } = 200;
    }

    /// <summary>Representa parâmetros de conexão do Redis.</summary>
    public class RedisSettings
    {
        public string ConnectionString { get; set; } = string.Empty;

        public string KeyPrefix { get; set; } = "authcore";
    }
}
